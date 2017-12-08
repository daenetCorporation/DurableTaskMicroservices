using Daenet.DurableTask.Microservices;
using DurableTask.Core;
using DurableTask.ServiceBus;
using DurableTask.ServiceBus.Tracking;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using Daenet.DurableTaskMicroservices.Host;

namespace WindowsServiceHost
{
    public partial class WindowServiceHost : ServiceBase
    {

        #region Private Member variables

        private EventLog m_ELog;
        private static string m_ServiceBusConnectionString;
        private static string m_StorageConnectionString;
        private static string m_SqlStateProviderConnectionString;
        private static string m_TaskHubName;
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
            try
            {
#if !DEVELOPMENT
            m_ELog = new EventLog("System", ".", "Daenet.DurableTask.Microservices");
#endif

                m_ELog?.WriteEntry("Started", EventLogEntryType.Information, 1);

                readConfiguration();

                AzureTableInstanceStore instanceStore = new AzureTableInstanceStore(m_TaskHubName, m_StorageConnectionString);
                ServiceBusOrchestrationService orchestrationServiceAndClient =
                   new ServiceBusOrchestrationService(m_ServiceBusConnectionString, m_TaskHubName, instanceStore, null, null);

                orchestrationServiceAndClient.CreateIfNotExistsAsync().Wait();

                TaskHubClient taskHubClient = new TaskHubClient(orchestrationServiceAndClient);
                TaskHubWorker taskHub = new TaskHubWorker(orchestrationServiceAndClient);

                ServiceHost host;

                host = new ServiceHost(orchestrationServiceAndClient, orchestrationServiceAndClient, instanceStore, false);

                var runningInstances = instanceStore.GetRunningInstances();

                host.StartServiceHostAsync(Environment.CurrentDirectory, runningInstances: runningInstances).Wait();
            }
            catch (Exception ex)
            {
                m_ELog?.WriteEntry($"Error: {ex.ToString()}", EventLogEntryType.Error, 1);
                this.Stop();
            }
        }

        #endregion


        private static void readConfiguration()
        {
            m_ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["ServiceBus"]?.ConnectionString;

            if (string.IsNullOrEmpty(m_ServiceBusConnectionString))
            {
                throw new Exception("A ServiceBus connection string must be defined in either an environment variable or in configuration.");
            }

            m_StorageConnectionString = ConfigurationManager.ConnectionStrings["Storage"]?.ConnectionString;
            m_SqlStateProviderConnectionString = ConfigurationManager.ConnectionStrings["SqlStateProviderConnectionString"]?.ConnectionString;
       
            m_TaskHubName = ConfigurationManager.AppSettings.Get("TaskHubName");
        }


        //private List<OrchestrationState> getRunningInstances(AzureTableInstanceStore instanceStore)
        //{
        //    List<OrchestrationState> instances = new List<OrchestrationState>();

        //    var byNameQuery = new OrchestrationStateQuery();
        //    byNameQuery.AddStatusFilter(OrchestrationStatus.Running);

        //    instances.AddRange( getInstancesByState(instanceStore, OrchestrationStatus.Running));

        //    instances.AddRange(getInstancesByState(instanceStore, OrchestrationStatus.ContinuedAsNew));

        //    instances.AddRange(getInstancesByState(instanceStore, OrchestrationStatus.Pending));

        //    return instances;
        //}

        //private IEnumerable<OrchestrationState> getInstancesByState(AzureTableInstanceStore instanceStore, OrchestrationStatus status)
        //{
        //    List<OrchestrationState> instances = new List<OrchestrationState>();

        //    var byNameQuery = new OrchestrationStateQuery();
        //    byNameQuery.AddStatusFilter(status);

        //    return instanceStore.QueryOrchestrationStatesAsync(byNameQuery).Result;
        //}
    }
}
