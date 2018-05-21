using DurableTask;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Core
{

    /// <summary>
    /// Implements the client functionality for DTF microservices.
    /// </summary>
    public class ServiceClient : MicroserviceBase
    {
        /// <summary>
        /// Creates the instance of ServiceClient, which can be used to act with microservice.
        /// </summary>
        /// <param name="sbConnStr"></param>
        /// <param name="storageConnStr"></param>
        /// <param name="hubName"></param>
        public ServiceClient(IOrchestrationServiceClient orchestrationClient)
        {
            this.m_HubClient = new TaskHubClient(orchestrationClient);
        }
        
        /// <summary>
        /// Forcefully terminate the specified orchestration instance
        /// </summary>
        /// <param name="svcInst"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public async Task TerminateAsync(MicroserviceInstance svcInst, string reason = "Terminated by host.")
        {
            await m_HubClient.TerminateInstanceAsync(svcInst.OrchestrationInstance, reason);
        }


        /// <summary>
        /// Raises an event in the specified orchestration instance, which eventually causes
        /// the OnEvent() method in the orchestration to fire.</summary>
        /// <param name="instance"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task RaiseEventAsync(MicroserviceInstance instance, string eventName, string data)
        {
            await m_HubClient.RaiseEventAsync(instance.OrchestrationInstance, eventName, data);
        }


    }
}
