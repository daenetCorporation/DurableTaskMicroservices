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
    public class ServiceClient
    {
        private TaskHubClient m_HubClient;
       

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
        /// Starts the new instance of the microservice by passing input arguments.
        /// This method will start the new instance of orchestration
        /// </summary>
        /// <param name="orchestrationQualifiedName">The full qualified name of orchestration to be started.</param>
        /// <param name="inputArgs">Input arguments.</param>
        /// <returns></returns>
        public async Task<MicroserviceInstance> StartServiceAsync(string orchestrationQualifiedName, object inputArgs, string version = "")
        {
            if (version == null)
                throw new ArgumentException("Value cannot be null!", nameof(version));

            var ms = new MicroserviceInstance()
            {
                OrchestrationInstance = await m_HubClient.CreateOrchestrationInstanceAsync(orchestrationQualifiedName, version, inputArgs),
            };
            return ms;
        }


        /// <summary>
        /// Starts the new instance of the microservice by passing input arguments.
        /// This method will start the new instance of orchestration
        /// </summary>
        /// <param name="orchestration">The type of orchestration to be started.</param>
        /// <param name="inputArgs">Input arguments.</param>
        /// <returns></returns>
        public async Task<MicroserviceInstance> StartServiceAsync(Type orchestration, object inputArgs)
        {
            var ms = new MicroserviceInstance()
            {
                OrchestrationInstance = await m_HubClient.CreateOrchestrationInstanceAsync(orchestration, inputArgs),
            };
            return ms;
        }

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
