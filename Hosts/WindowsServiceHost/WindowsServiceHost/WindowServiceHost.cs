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

        #region Private Member variables
        private StringBuilder m_Trace = new StringBuilder();

        private static string m_ServiceBusConnectionString;
        private static string m_StorageConnectionString;
        private static string m_SqlStateProviderConnectionString;
        private static string m_TaskHubName;

        private EventLog m_ELog;
        private static string m_SchemaName;

        #endregion

        #region Public/Protected Methods

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
            m_ELog?.WriteEntry("Stopped", EventLogEntryType.Information, 1);
        }

        internal void RunService()
        {
#if !DEVELOPMENT
            m_ELog = new EventLog("System", ".", "Daenet.DurableTask.Microservices");
#endif
            try
            {
                m_ELog?.WriteEntry("Started", EventLogEntryType.Information, 1);
                m_Trace.AppendLine("Service Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());

                string[] configFiles = loadConfigFiles();
                if (configFiles.Length > 0)
                {
                    m_Trace.AppendLine(String.Format("Loaded {0} configuration files.", configFiles.Length));

                    var host = createMicroserviceHost();

                    m_Trace.AppendLine("Host created successfully.");

                    List<Microservice> services = new List<Microservice>();

                    startServicesFromConfigFile(host, configFiles);
                }
                else
                {
                    m_ELog?.WriteEntry("No any *.config.xml file has been found in deployment folder " + Environment.CurrentDirectory,
                        EventLogEntryType.Warning, 2);

                    this.Stop();
                }

                m_ELog?.WriteEntry(m_Trace.ToString(), EventLogEntryType.Information, 3);
            }
            catch (Exception ex)
            {
                m_Trace.AppendLine("---------------");
                m_Trace.AppendLine("Error:");
                m_Trace.AppendLine(ex.ToString());

                m_ELog?.WriteEntry(m_Trace.ToString(), EventLogEntryType.Error, 4);

                this.Stop();
            }
        }

        #endregion

        #region Private Methods

        private void startServicesFromConfigFile(ServiceHost host, string[] cfgFiles)
        {
            var svcInstances = host.LoadServicesFromXml(cfgFiles, loadKnownTypes(), out ICollection<Microservice> services);

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
                    host.StartService(svc.OrchestrationQName, svc.InputArgument);
                    m_Trace.AppendLine(String.Format("Services {0} has been started.", svc));
                }
                else
                {
                    m_Trace.AppendLine(String.Format("{0} instance(s) of service {1} is(are) already running. No action performed", cnt, svc.OrchestrationQName));
                }
            }
        }

        /// <summary>
        /// Get all files which matches to *.config.xml
        /// </summary>
        /// <returns></returns>
        private string[] loadConfigFiles()
        {
            List<string> configFiles = new List<string>();

            foreach (var cfgFile in Directory.GetFiles(getBaseDirectory(), "*.config.xml"))
            {
                configFiles.Add(cfgFile);
            }

            return configFiles.ToArray();
        }

        /// <summary>
        /// Gets the location of service binary.
        /// </summary>
        /// <returns></returns>
        private string getBaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        private Type[] loadKnownTypes()
        {
            List<Type> types = new List<Type>();

            foreach (var assemblyFile in Directory.GetFiles(getBaseDirectory(), "*.dll", SearchOption.AllDirectories))
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

            return types.ToArray();
        }

        private ServiceHost createMicroserviceHost()
        {
            readConfiguration();

            m_Trace.AppendLine(String.Format("SB connection String: '{0}'\r\n Storage Connection String: '{1}', \r\nTaskHub: '{2}'",
                m_ServiceBusConnectionString, m_StorageConnectionString, m_TaskHubName));

            ServiceHost host;

            // StorageConnectionString exists and SqlStateProvider is null
            if (String.IsNullOrEmpty(m_SqlStateProviderConnectionString) && String.IsNullOrEmpty(m_StorageConnectionString) == false)
            {
                host = new ServiceHost(m_ServiceBusConnectionString, m_StorageConnectionString, m_TaskHubName);
            }
            else if (String.IsNullOrEmpty(m_SqlStateProviderConnectionString) == false)
            {
                Dictionary<string, object> services = new Dictionary<string, object>();
                //services.Add(DurableTask.TaskHubWorker.StateProviderKeyName, new DaenetSqlProvider(TaskHubName, SqlStateProviderConnectionString, m_SchemaName));
                throw new NotImplementedException("SQLStateProvider loading is not implemented atm!");

                host = new ServiceHost(m_ServiceBusConnectionString, m_StorageConnectionString, m_TaskHubName, false, services);
            }
            else
                throw new Exception("StorageConnectionString and SqlStateProviderConnectionString are not set. Please set one of them in AppSettings!");

            return host;
        }

        private static void readConfiguration()
        {
            m_ServiceBusConnectionString = ConfigurationManager.AppSettings["ServiceBusConnectionString"];

            if (string.IsNullOrEmpty(m_ServiceBusConnectionString))
            {
                throw new Exception("A ServiceBus connection string must be defined in either an environment variable or in configuration.");
            }

            m_StorageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            m_SqlStateProviderConnectionString = ConfigurationManager.AppSettings["SqlStateProviderConnectionString"];
            m_SchemaName = ConfigurationManager.AppSettings["SqlStateProviderConnectionString.SchemaName"];

            if (string.IsNullOrEmpty(m_StorageConnectionString) && String.IsNullOrEmpty(m_SqlStateProviderConnectionString))
            {
                throw new Exception("A Storage connection string must be defined in either an environment variable or in configuration.");
            }

            m_TaskHubName = ConfigurationManager.AppSettings.Get("TaskHubName");
        }

        #endregion
    }
}
