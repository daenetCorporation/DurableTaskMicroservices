//  ----------------------------------------------------------------------------------
//  Copyright daenet Gesellschaft für Informationstechnologie mbH
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  ----------------------------------------------------------------------------------

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
        /// <param name="directory">Directory where to search for *.config.xml, *.config.json and assemblies</param>
        public void StartServiceHost(string directory = null, string searchPattern = "*.config.xml")
        {
            try
            {
                m_Logger?.LogInformation("Service started. Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());

                if (String.IsNullOrEmpty(directory))
                    directory = Environment.CurrentDirectory;

                string[] configFiles = loadConfigFiles(directory, searchPattern);

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
                    m_Logger?.LogInformation("No {searchPattern} files found in folder: {folder}.", searchPattern, directory);
                    throw new Exception(String.Format("No {0} files found in folder: {1}.", searchPattern, directory));
                }
            }
            catch (Exception ex)
            {
                m_Logger?.LogError(ex, "Failed to start the Host.");

                throw;
            }
        }

        #region Private Methods

        private bool m_IsJson = false;

        private void startServicesFromConfigFile(ServiceHost host, string[] cfgFiles, string directory)
        {
            ICollection<Microservice> services;

            ICollection<MicroserviceInstance> svcInstances;

            List<Type> knownTypes = new List<Type>();

            if (m_IsJson)
            {
                throw new Exception("JSON loading is not supported at the moment.");
                // TODO..
                //svcInstances = host.LoadServiceFromJson(cfgFiles.FirstOrDefault(), out services);
            }
            else
            {
                knownTypes.AddRange(loadKnownTypes(directory));
                if (directory != AppContext.BaseDirectory)
                    knownTypes.AddRange(loadKnownTypes(AppContext.BaseDirectory));

                svcInstances = host.LoadServicesFromXml(cfgFiles, knownTypes, out services);
            }

            m_Logger?.LogInformation("{0} service(s) have been registered on Service Bus hub", services.Count);

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
                    if (svc.IsSingletone)
                    {
                        host.StartService(svc.OrchestrationQName, svc.InputArgument);
                        m_Logger?.LogInformation("Service {0} has been started.", svc);
                    }
                    else
                        m_Logger?.LogInformation("Service {0} is not declared as singleton. It is loaded, but not started.", svc);
                }
                else
                {
                    m_Logger?.LogInformation("{0} instance(s) of service {1} is(are) already running. No action performed", cnt, svc.OrchestrationQName);
                }
            }
        }

        /// <summary>
        /// Get all files which matches to *.config.xml
        /// </summary>
        /// <returns></returns>
        private string[] loadConfigFiles(string directory, string searchPattern = "*.config.xml")
        {
            List<string> configFiles = new List<string>();

            foreach (var cfgFile in Directory.GetFiles(directory, searchPattern))
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

            m_Logger?.LogInformation("SB connection String: '{0}'\r\n Storage Connection String: '{1}', \r\nTaskHub: '{2}'",
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
            {
                host = new ServiceHost(m_ServiceBusConnectionString, m_TaskHubName);
                //throw new Exception("StorageConnectionString and SqlStateProviderConnectionString are not set. Please set one of them in AppSettings!");
            }

            return host;
        }

        private static void readConfiguration()
        {
            m_ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["ServiceBus"]?.ConnectionString;

            if (string.IsNullOrEmpty(m_ServiceBusConnectionString))
            {
                throw new Exception("A ServiceBus connection string must be defined in either an environment variable or in configuration.");
            }

            m_StorageConnectionString = ConfigurationManager.ConnectionStrings["Storage"]?.ConnectionString;
            m_SqlStateProviderConnectionString = ConfigurationManager.ConnectionStrings["SqlStateProviderConnectionString"]?.ConnectionString;
            m_SchemaName = ConfigurationManager.AppSettings["SqlStateProviderConnectionString.SchemaName"];

            //if (string.IsNullOrEmpty(m_StorageConnectionString) && String.IsNullOrEmpty(m_SqlStateProviderConnectionString))
            //{
            //    throw new Exception("A Storage connection string must be defined in either an environment variable or in configuration.");
            //}

            m_TaskHubName = ConfigurationManager.AppSettings.Get("TaskHubName");
        }

#endregion
    }
}
