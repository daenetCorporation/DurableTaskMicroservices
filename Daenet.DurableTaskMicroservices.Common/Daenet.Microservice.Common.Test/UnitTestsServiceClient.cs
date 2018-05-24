using Daenet.DurableTaskMicroservices.Common.Test.HelloWorldOrchestration;
using Daenet.DurableTaskMicroservices.Core;
using Daenet.DurableTaskMicroservices.Host;
using DurableTask.Core;
using DurableTask.Core.Tracing;
using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;

namespace Daenet.Microservice.Common.Test
{
    /// <summary>
    /// Tests for <see cref="ServiceClient" functionality./>
    /// </summary>
    [TestClass]
    public class UnitTestsServiceClient
    {
        private static string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString;
        private static string StorageConnectionString = ConfigurationManager.ConnectionStrings["SqlStorage"].ConnectionString;

        /// <summary>
        /// Full qualified name of orchestration has to be specified. Note, we do not need a reference to orchestration imlementation
        /// in order to be able to start it.
        /// </summary>
        private const string cSvcName = "Daenet.Microservice.Common.Test.HelloWorldOrchestration.HelloWorldOrchestration";

        private static ILoggerFactory getDebugLoggerFactory()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug(LogLevel.Trace);

            return loggerFactory;
        }

        static ObservableEventListener eventListener = new ObservableEventListener();

        /// <summary>
        /// Starts the hub, loads orchestration, starts orchestration, waits on orchestration to complete.
        /// </summary>
        [TestMethod]
        public void SelfHostServiceClientTest()
        {
            eventListener.LogToConsole();
            eventListener.EnableEvents(DefaultEventSource.Log, EventLevel.Verbose);
            //eventListener.EnableEvents(null, EventLevel.Verbose)
            //eventListener.Subscribe((a)=> a    


            var loggerFact = DurableTaskMicroservices.Common.Test.UnitTestLogging.GetDebugLoggerFactory();

            List<OrchestrationState> runningInstances;

            ServiceHost host = HostHelpersExtensions.CreateMicroserviceHost(ServiceBusConnectionString, StorageConnectionString, nameof(SelfHostServiceClientTest), true, out runningInstances, loggerFact);

            var microservices = host.StartServiceHostAsync(Path.Combine(), runningInstances: runningInstances, context: new Dictionary<string, object>() { { "company", "daenet" } }).Result;

            // var r = new Daenet.Microservice() { };

            //xmlSerializeService(typeof(Daenet.DurableTaskMicroservices.UnitTests.CounterOrchestration), "aaa.xml");

            ServiceClient client = ClientHelperExtensions.CreateMicroserviceClient(ServiceBusConnectionString, StorageConnectionString, nameof(SelfHostServiceClientTest));

            var svc = client.StartServiceAsync(cSvcName, new HelloWorldOrchestrationInput { HelloText = "SelfHostServiceClientTestInputArg" }).Result;

            microservices.Add(svc);

            host.WaitOnInstances(host, microservices);
        }


        /// <summary>
        /// Executes two orchestrations. First one is loaded from host and configuration and second one 
        /// HelloWorldOrchetration is started from client with invalid arguments.
        /// Invalid arguments will couse internal server errors, which have to be received by this test
        /// inside of method SubscribeEvents.
        /// </summary>
        [TestMethod]
        public void ServiceEventsTest()
        {
            int errCnt = 0;

            var loggerFact = DurableTaskMicroservices.Common.Test.UnitTestLogging.GetDebugLoggerFactory(LogLevel.None);

            List<OrchestrationState> runningInstances;

            ServiceHost host = HostHelpersExtensions.CreateMicroserviceHost(ServiceBusConnectionString, DurableTaskMicroservices.Common.Test.UnitTestLogging.SqlStorageConnectionString, nameof(ServiceEventsTest), true, out runningInstances, loggerFact);

            //
            // This method subscribes all errors, which happen internally on host.
            host.SubscribeEvents(EventLevel.LogAlways,
                (msg) =>
                {
                    Debug.WriteLine(msg);
                    if (msg.Contains("Error converting value \"invalid input\" to type"))
                        errCnt++;

                }, "errors");

            var microservices = host.StartServiceHostAsync(Path.Combine(), runningInstances: runningInstances, context: new Dictionary<string, object>() { { "company", "daenet" } }).Result;

            string svcName = "Daenet.Microservice.Common.Test.HelloWorldOrchestration.HelloWorldOrchestration";

            var svc = host.StartServiceAsync(svcName, "invalid input").Result;

            microservices.Add(svc);

            host.WaitOnInstances(host, microservices);

            Assert.IsTrue(errCnt > 0);
        }

        [TestMethod]
        public void SelfHostServiceClientTestWithHubName()
        {
            var loggerFact = DurableTaskMicroservices.Common.Test.UnitTestLogging.GetDebugLoggerFactory();

            ServiceClient client = ClientHelperExtensions.CreateMicroserviceClient(ServiceBusConnectionString, StorageConnectionString, nameof(SelfHostServiceClientTest));

            var orchestrationInput = new HelloWorldOrchestrationInput
            {
                HelloText = "SelfHostServiceClientTestInputArg",
                Context = new Dictionary<string, object> { { "ActivityId", "SelfHostServiceClientTestWithHubName" } }
            };

            var svc = client.StartServiceAsync(cSvcName, orchestrationInput).Result;

            Assert.IsTrue(svc != null);
        }
    }
}
