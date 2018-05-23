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
