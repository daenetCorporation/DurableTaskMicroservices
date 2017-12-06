using Daenet.DurableTaskMicroservices.UnitTests;
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

namespace Daenet.DurableTaskMicroservices.DotNetUnitTests
{
    [TestClass]
    public class DtfNativeTests
    {
        private static string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString;
        private static string StorageConnectionString = ConfigurationManager.ConnectionStrings["Storage"].ConnectionString;
        private static string TaskHubName = ConfigurationManager.AppSettings["TaskHubName"];

      
        [TestMethod]
        public void OpenAndStartServiceHostTest2()
        {

        }

        static ObservableEventListener eventListener;

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

            var rnts = instanceStore.GetJumpStartEntitiesAsync(1000).Result;

            var instance = taskHubClient.CreateOrchestrationInstanceAsync(typeof(CounterOrchestration), Guid.NewGuid().ToString(), new TestOrchestrationInput()).Result;
            
            taskHub.StartAsync().Wait();

            taskHubClient.WaitForOrchestrationAsync(instance, TimeSpan.MaxValue).Wait();

            var state = instanceStore.GetOrchestrationStateAsync(instance.InstanceId, true).Result;

            var res = taskHubClient.GetOrchestrationHistoryAsync(instance).Result;

            Thread.Sleep(int.MaxValue);
        }
    }
}
