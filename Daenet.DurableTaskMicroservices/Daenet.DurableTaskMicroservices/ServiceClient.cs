using DurableTask;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daenet.DurableTask.Microservices
{

    /// <summary>
    /// Implements the client functionality for DTF microservices.
    /// </summary>
    public class ServiceClient
    {
        private TaskHubClient m_HubClient;
       
        private string m_ServiceBusConnectionString;
        private string m_StorageConnectionString;
        private string m_TaskHubName;


        /// <summary>
        /// Creates the instance of ServiceClient, which can be used to act with microservice.
        /// </summary>
        /// <param name="sbConnStr"></param>
        /// <param name="storageConnStr"></param>
        /// <param name="hubName"></param>
        public ServiceClient(string sbConnStr, string storageConnStr, string hubName)
        {
            this.m_ServiceBusConnectionString = sbConnStr;
            this.m_StorageConnectionString = storageConnStr;
            this.m_TaskHubName = hubName;
        
           this.m_HubClient = createTaskHubClient(!String.IsNullOrEmpty(storageConnStr));
        }

        public TaskHubClient createTaskHubClient(bool createInstanceStore = true)
        {
            var settings = ServiceHost.GetDefaultHubClientSettings();          

            if (createInstanceStore)
            {
                return new TaskHubClient(m_TaskHubName, m_ServiceBusConnectionString, m_StorageConnectionString, settings);
            }
            else
                return new TaskHubClient(m_TaskHubName, m_ServiceBusConnectionString);
        }


        /// <summary>
        /// Starts the new instance of the microservice by passing input arguments.
        /// This method will start the new instance of orchestration
        /// </summary>
        /// <param name="orchestrationQualifiedName">The full qualified name of orchestration to be started.</param>
        /// <param name="inputArgs">Input arguments.</param>
        /// <returns></returns>
        public MicroserviceInstance StartService(string orchestrationQualifiedName, object inputArgs)
        {
            return StartService(Type.GetType(orchestrationQualifiedName), inputArgs);
        }


        /// <summary>
        /// Starts the new instance of the microservice by passing input arguments.
        /// This method will start the new instance of orchestration
        /// </summary>
        /// <param name="orchestration">The type of orchestration to be started.</param>
        /// <param name="inputArgs">Input arguments.</param>
        /// <returns></returns>
        public MicroserviceInstance StartService(Type orchestration, object inputArgs)
        {
            return createServiceInstance(orchestration, inputArgs);
        }

        private MicroserviceInstance createServiceInstance(Type orchestration, object inputArgs)
        {
            var ms = new MicroserviceInstance()
            {
                OrchestrationInstance = m_HubClient.CreateOrchestrationInstance(orchestration, inputArgs),
            };
            return ms;
        }
    }
}
