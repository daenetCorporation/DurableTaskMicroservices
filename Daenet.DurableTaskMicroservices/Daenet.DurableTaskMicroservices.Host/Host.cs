using Daenet.DurableTask.Microservices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Daenet.DurableTaskMicroservices.Host
{
    public class Host
    {
        #region Private Member variables

        private static string m_ServiceBusConnectionString;
        private static string m_StorageConnectionString;
        private static string m_SqlStateProviderConnectionString;
        private static string m_TaskHubName;

        private ILogger m_Logger;
        private static string m_SchemaName;
        private ILoggerFactory m_LoggerFactory;

        #endregion

        public Host(ILoggerFactory loggerFactory)
        {
            m_LoggerFactory = loggerFactory;
            m_Logger = m_LoggerFactory.CreateLogger<Host>();
        }

        public Host()
        {

        }

        /// <summary>
        /// Starts the MicroService Host
        /// </summary>
        /// <param name="directory">Directory where to search for *.config.xml/assemblies</param>
        public void StartServiceHost(string directory = null)
        {
            try
            {
                m_Logger?.LogInformation("Service started. Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());

                if (String.IsNullOrEmpty(directory))
                    directory = Environment.CurrentDirectory;

                string[] configFiles = loadConfigFiles(directory);

                if (configFiles.Length > 0)
                {
                    m_Logger?.LogInformation("Loaded {0} configuration files.", configFiles.Length);

                    var host = createMicroserviceHost();

                    m_Logger?.LogInformation("Host created successfully.");

                    List<Microservice> services = new List<Microservice>();

                    startServicesFromConfigFile(host, configFiles, directory);
                }
                else
                {
                    m_Logger?.LogInformation("No *.config.xml files found in folder: {folder}.", directory);
                    throw new Exception(String.Format("No *.config.xml files found in folder: {0}.", directory));
                }
            }
            catch (Exception ex)
            {
                m_Logger?.LogError(ex, "Failed to start the Host.");

                throw;
            }
        }

        #region Private Methods

        private void startServicesFromConfigFile(ServiceHost host, string[] cfgFiles, string directory)
        {
            var svcInstances = host.LoadServicesFromXml(cfgFiles, loadKnownTypes(directory), out ICollection<Microservice> services);

            m_Logger.LogInformation("{0} service(s) have been registered on Service Bus hub", services.Count);

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
                    m_Logger.LogInformation("Services {0} has been started.", svc);
                }
                else
                {
                    m_Logger.LogInformation("{0} instance(s) of service {1} is(are) already running. No action performed", cnt, svc.OrchestrationQName);
                }
            }
        }

        /// <summary>
        /// Get all files which matches to *.config.xml
        /// </summary>
        /// <returns></returns>
        private string[] loadConfigFiles(string directory)
        {
            List<string> configFiles = new List<string>();

            foreach (var cfgFile in Directory.GetFiles(directory, "*.config.xml"))
            {
                configFiles.Add(cfgFile);
            }

            return configFiles.ToArray();
        }

        private Type[] loadKnownTypes(string directory)
        {
            List<Type> types = new List<Type>();

            foreach (var assemblyFile in Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories))
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

            m_Logger.LogInformation("SB connection String: '{0}'\r\n Storage Connection String: '{1}', \r\nTaskHub: '{2}'",
                m_ServiceBusConnectionString, m_StorageConnectionString, m_TaskHubName);

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
                throw new NotImplementedException("SQLStateProvider loading is not implemented!");

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
