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
using Daenet.DurableTaskMicroservices.Tests.TaskOrchestration.CounterOrchestration;
using DurableTask.Core;
using DurableTask.Core.Tracing;
using DurableTask.ServiceBus;
using DurableTask.ServiceBus.Tracking;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Tests
{
    /// <summary>
    /// Set of tests without dependency to microservice framework.
    /// </summary>
    [TestClass]
    public class DtfNativeTests
    {
        private static string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString;
        private static string StorageConnectionString = ConfigurationManager.ConnectionStrings["Storage"].ConnectionString;
        private static string TaskHubName = ConfigurationManager.AppSettings["TaskHubName"];

    
        static ObservableEventListener eventListener;

        /// <summary>
        /// This test does not uses any daenet Microservice functionality.
        /// It runs DTF orchestration nativelly to ensure that is all setup correctlly.
        /// </summary>
        [TestMethod]
        public void OpenAndStartServiceHostTest()
        {
            eventListener = new ObservableEventListener();
            eventListener.LogToConsole();
            eventListener.EnableEvents(DefaultEventSource.Log, EventLevel.LogAlways);

            TraceSource source = new TraceSource("DurableTask");
            source.Listeners.AddRange(Trace.Listeners);

            IOrchestrationServiceInstanceStore instanceStore = new AzureTableInstanceStore(TaskHubName, StorageConnectionString);
             ServiceBusOrchestrationService orchestrationServiceAndClient =
                new ServiceBusOrchestrationService(ServiceBusConnectionString, TaskHubName, instanceStore, null, null);

            orchestrationServiceAndClient.CreateIfNotExistsAsync().Wait();

            TaskHubClient taskHubClient = new TaskHubClient(orchestrationServiceAndClient);
            TaskHubWorker taskHub = new TaskHubWorker(orchestrationServiceAndClient);

            taskHub.AddTaskOrchestrations(typeof(CounterOrchestration));
            taskHub.AddTaskActivities(typeof(Task1), typeof(Task2));

            var rnts = ((AzureTableInstanceStore)instanceStore).GetJumpStartEntitiesAsync(1000).Result;

            var byNameQuery = new OrchestrationStateQuery();
            byNameQuery.AddStatusFilter(OrchestrationStatus.Pending);
            byNameQuery.AddNameVersionFilter(typeof(CounterOrchestration).FullName);

            var results = ((AzureTableInstanceStore)instanceStore).QueryOrchestrationStatesAsync(byNameQuery).Result;

            var instance = taskHubClient.CreateOrchestrationInstanceAsync(typeof(CounterOrchestration), Guid.NewGuid().ToString(), new TestOrchestrationInput()).Result;
            
            taskHub.StartAsync().Wait();
                   
            var state = instanceStore.GetOrchestrationStateAsync(instance.InstanceId, true).Result;

            var res = taskHubClient.GetOrchestrationHistoryAsync(instance).Result;

            taskHubClient.WaitForOrchestrationAsync(instance, TimeSpan.MaxValue).Wait();
        }
    }
}
