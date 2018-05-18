using DurableTask;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTask.Microservices
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

      


        ///// <summary>
        ///// Starts the new instance of the microservice by passing input arguments.
        ///// This method will start the new instance of orchestration
        ///// </summary>
        ///// <param name="orchestration">The type of orchestration to be started.</param>
        ///// <param name="inputArgs">Input arguments.</param>
        ///// <returns></returns>
        //public async Task<MicroserviceInstance> StartServiceAsync(Type orchestration, object inputArgs)
        //{
        //    var ms = new MicroserviceInstance()
        //    {
        //        OrchestrationInstance = await m_HubClient.CreateOrchestrationInstanceAsync(orchestration, inputArgs),
        //    };
        //    return ms;
        //}

        public async Task Terminate(MicroserviceInstance svcInst, string reason = "Terminated by host.")
        {
            await m_HubClient.TerminateInstanceAsync(svcInst.OrchestrationInstance, reason);
        }

        public async Task TerminateAsync(MicroserviceInstance svcInst, string reason = "Terminated by host.")
        {
            await m_HubClient.TerminateInstanceAsync(svcInst.OrchestrationInstance, reason);
        }


        public async Task RaiseEvent(MicroserviceInstance instance, string eventName, string data)
        {
            await m_HubClient.RaiseEventAsync(instance.OrchestrationInstance, eventName, data);
        }

     
    }
}
