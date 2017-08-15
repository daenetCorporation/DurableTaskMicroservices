using Daenet.DurableTask.Microservices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WindowsServiceHost
{
    public partial class WindowServiceHost : ServiceBase
    {
        private StringBuilder m_Trace = new StringBuilder();

        private static string ServiceBusConnectionString;
        private static string StorageConnectionString;
        private static string SqlStateProviderConnectionString;
        private static string TaskHubName;

        private EventLog m_ELog;
        private static string m_SchemaName;

        public WindowServiceHost()
        {
            InitializeComponent();
        }


        protected override void OnStart(string[] args)
        {
            RunService();
        }

        protected override void OnStop()
        {
            m_ELog.WriteEntry("Stopped", EventLogEntryType.Information, 1);
        }



        internal void RunService()
        {
            //int a = 1;
            //while (a > 0)
            //{
            //    Thread.Sleep(2500);

            //}

            m_ELog = new EventLog("System", ".", "Dtf");

            try
            {
                m_ELog.WriteEntry("Started", EventLogEntryType.Information, 1);
                m_Trace.AppendLine("Service Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());

                string[] configFiles = loadConfigFiles();
                if (configFiles.Length > 0)
                {
                    m_Trace.AppendLine(String.Format("Loaded {0} configuration files.", configFiles.Length));

                    var host = createMicroserviceHost();

                    m_Trace.AppendLine("Host created successfully.");

                    List<Microservice> services = new List<Microservice>();

                    foreach (var cfgFile in configFiles)
                    {
                        var service = deserializeService(cfgFile);
                        services.Add(service);
                        m_Trace.AppendLine(String.Format("Service configuration loaded from '{0}'", cfgFile));
                    }

                    startServicesFromConfigFile(host, services);
                }
                else
                {
                    m_ELog.WriteEntry("No any *.config.xml file has been found in deployment folder " + Environment.CurrentDirectory,
                        EventLogEntryType.Warning, 2);

                    this.Stop();
                }

                m_ELog.WriteEntry(m_Trace.ToString(), EventLogEntryType.Information, 3);
            }
            catch (Exception ex)
            {
                m_Trace.AppendLine("");
                m_Trace.AppendLine("---------------");
                m_Trace.AppendLine("Error:");
                m_Trace.AppendLine(ex.Message);
                m_Trace.AppendLine(getInnerExceptionText(ex));
                m_Trace.AppendLine("");
                m_Trace.AppendLine("Stack Trace:");
                m_Trace.AppendLine(ex.StackTrace);

                m_ELog.WriteEntry(m_Trace.ToString(), EventLogEntryType.Error, 4);

                this.Stop();
            }
        }

        private string getInnerExceptionText(Exception ex)
        {
            string text = "";
            if (ex != null)
            {
                if (ex.InnerException != null)
                {
                    text = ex.InnerException.Message;
                    text += getInnerExceptionText(ex.InnerException);
                }
            }

            return text;
        }

        private void startServicesFromConfigFile(ServiceHost host, List<Microservice> services)
        {
            host.LoadServices(services.ToArray());
            m_Trace.AppendLine(String.Format("{0} service(s) have been registered on Service Bus hub", services.Count));

            bool isStarted = false;

            foreach (var svc in services)
            {
                int cnt = host.GetNumOfRunningInstances(svc);

                if (isStarted == false)
                {
                    host.Open();
                    isStarted = true;
                }

                if (cnt == 0)
                {
                    host.StartService(svc.OrchestrationQName, (OrchestrationInput)svc.InputArgument, null);
                    m_Trace.AppendLine(String.Format("Services {0} has been started.", svc));
                }
                else
                {
                    m_Trace.AppendLine(String.Format("{0} instance(s) of service {1} is(are) already running. No action performed", cnt, svc.OrchestrationQName));
                }
            }
        }

        private string[] loadConfigFiles()
        {
            List<string> configFiles = new List<string>();

            foreach (var cfgFile in Directory.GetFiles(getActiveDirectory(), "*.config.xml"))
            {
                configFiles.Add(cfgFile);
            }

            return configFiles.ToArray();
        }

        /// <summary>
        /// Gets the location of service binary.
        /// </summary>
        /// <returns></returns>
        private string getActiveDirectory()
        {
            //   Debugger.Break();
            //int s = 1;
            //while (s > 0)
            //{
            //    Thread.Sleep(2500);
            //}

            return AppDomain.CurrentDomain.BaseDirectory;
        }



        private Microservice deserializeService(string configFile)
        {
            m_ELog.WriteEntry(String.Format("DeserializeService config: {0}", configFile), EventLogEntryType.Information, 1);

            using (XmlReader writer = XmlReader.Create(configFile))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(Microservice), loadKnownTypes());
                object svc = ser.ReadObject(writer);
                return svc as Microservice;
            }
        }

        private void serializeService(Microservice svc)
        {
            using (XmlWriter writer = XmlWriter.Create("abc.xml"))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(Microservice), loadKnownTypes());
                ser.WriteObject(writer, (Microservice)svc);
            }
        }


        private Type[] loadKnownTypes()
        {
            List<Type> types = new List<Type>();

            foreach (var assemblyFile in Directory.GetFiles(getActiveDirectory(), "*.dll", SearchOption.AllDirectories))
            {
                //if (assemblyFile.Contains("Igus.Integration.LockTypeInterfaces.dll"))
                {
                    Assembly asm = Assembly.LoadFile(assemblyFile);
                    var attr = asm.GetCustomAttribute(typeof(IntegrationAssemblyAttribute));
                    if (attr != null)
                    {
                        foreach (var type in asm.GetTypes())
                        {
                            if (type.GetCustomAttributes(typeof(DataContractAttribute)).Count() > 0)
                            {
                                types.Add(type);
                            }
                        }
                    }
                }
            }

            return types.ToArray();
        }

        private ServiceHost createMicroserviceHost()
        {
            readConfiguration();

            m_Trace.AppendLine(String.Format("SB connection String: '{0}'\r\n Storage Connection String: '{1}', \r\nTaskHub: '{2}'",
                ServiceBusConnectionString, StorageConnectionString, TaskHubName));

            ServiceHost host;

            // StorageConnectionString exists and SqlStateProvider is null
            if (String.IsNullOrEmpty(SqlStateProviderConnectionString) && String.IsNullOrEmpty(StorageConnectionString) == false)
            {
                host = new ServiceHost(ServiceBusConnectionString, StorageConnectionString, TaskHubName);
            }
            else if (String.IsNullOrEmpty(SqlStateProviderConnectionString) == false)
            {
                Dictionary<string, object> services = new Dictionary<string, object>();
                //services.Add(DurableTask.TaskHubWorker.StateProviderKeyName, new DaenetSqlProvider(TaskHubName, SqlStateProviderConnectionString, m_SchemaName));

                host = new ServiceHost(ServiceBusConnectionString, StorageConnectionString, TaskHubName, false, services);
            }
            else
                throw new Exception("StorageConnectionString and SqlStateProviderConnectionString are not set. Please set one of them in AppSettings!");

            return host;
        }


        private static void readConfiguration()
        {
                ServiceBusConnectionString = ConfigurationManager.AppSettings["ServiceBusConnectionString"];

            if (string.IsNullOrEmpty(ServiceBusConnectionString))
            {
                throw new Exception("A ServiceBus connection string must be defined in either an environment variable or in configuration.");
            }

            StorageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            SqlStateProviderConnectionString = ConfigurationManager.AppSettings["SqlStateProviderConnectionString"];
            m_SchemaName = ConfigurationManager.AppSettings["SqlStateProviderConnectionString.SchemaName"];

            if (string.IsNullOrEmpty(StorageConnectionString) || String.IsNullOrEmpty(SqlStateProviderConnectionString))
            {
                throw new Exception("A Storage connection string must be defined in either an environment variable or in configuration.");
            }

                TaskHubName = ConfigurationManager.AppSettings.Get("TaskHubName");
        }
    }
}
