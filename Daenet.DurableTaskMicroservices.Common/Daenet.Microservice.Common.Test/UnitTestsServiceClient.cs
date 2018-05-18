using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.Extensions;
using Daenet.DurableTaskMicroservices.Host;
using DurableTask.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Daenet.Common.Logging.Sql;
using Daenet.DurableTaskMicroservices.UnitTests;
using System.Runtime.Serialization;
using System.Xml;
using System;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
using DurableTask.Core.Tracing;
using System.Diagnostics.Tracing;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;

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

        private static void xmlSerializeService(Daenet.DurableTask.Microservices.Microservice svc, string fileName)
        {
            using (XmlWriter writer = XmlWriter.Create(fileName, new System.Xml.XmlWriterSettings() { Indent = true }))
            {
                DataContractSerializerSettings sett = new DataContractSerializerSettings();
                DataContractSerializer ser = new System.Runtime.Serialization.DataContractSerializer(typeof(Daenet.DurableTask.Microservices.Microservice),
                    loadKnownTypes());
                ser.WriteObject(writer, (Daenet.DurableTask.Microservices.Microservice)svc);
            }
        }

        private static Type[] loadKnownTypes()
        {
            List<Type> types = new List<Type>();

            string[] patterns = new string[] { "*.dll", "*.exe" };

            foreach (var pattern in patterns)
            {
                Assembly asm = typeof(Daenet.DurableTaskMicroservices.UnitTests.CounterOrchestration).Assembly;

                foreach (var type in asm.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(DataContractAttribute)).Count() > 0)
                    {
                        types.Add(type);
                    }
                }
            }

            return types.ToArray();
        }


        static ObservableEventListener eventListener = new ObservableEventListener();

        /// <summary>
        /// Starts the hub, loads orchestration, starts orchestration, waits on orchestration to complete.
        /// </summary>
        [TestMethod]
        public void SelfHostServiceClientTest()
        {
            //var ms = new Daenet.DurableTask.Microservices.Microservice()
            //{
            //    AutoStart = true,
            //    ActivityQNames = new string[]
            //      {
            //          typeof(Task1).AssemblyQualifiedName,
            //          typeof(Task2).AssemblyQualifiedName
            //      },
            //     InputArgument = new Daenet.DurableTaskMicroservices.UnitTests.CounterOrchestrationInput()
            //     {
            //          Counter = 3, Delay = 1000
            //     },
            //      OrchestrationQName = typeof(Daenet.DurableTaskMicroservices.UnitTests.CounterOrchestration).AssemblyQualifiedName,
            //};

            //xmlSerializeService(ms, "aaa.xml");

            //            DefaultEventSource.Log

            //Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener().TraceOutputOptions.
            //EventSourceAnalyzer.InspectAll(DefaultEventSource.Log);

            eventListener.LogToConsole();
            eventListener.EnableEvents(DefaultEventSource.Log, EventLevel.Verbose);
            //eventListener.EnableEvents(null, EventLevel.Verbose)
            //eventListener.Subscribe((a)=> a    


            var loggerFact = UnitTestLogging.GetDebugLoggerFactory();

            List<OrchestrationState> runningInstances;

            ServiceHost host = HostHelpersExtensions.CreateMicroserviceHost(ServiceBusConnectionString, StorageConnectionString, nameof(SelfHostServiceClientTest), true, out runningInstances, loggerFact);

            var microservices = host.StartServiceHostAsync(Path.Combine(), runningInstances: runningInstances, context: new Dictionary<string, object>() { { "company", "daenet" } }).Result;
            
           // var r = new Daenet.Microservice() { };

            //xmlSerializeService(typeof(Daenet.DurableTaskMicroservices.UnitTests.CounterOrchestration), "aaa.xml");

            ServiceClient client = ClientHelperExtensions.CreateMicroserviceClient(ServiceBusConnectionString, StorageConnectionString, nameof(SelfHostServiceClientTest));

            var svc = client.StartServiceAsync(cSvcName, new HelloWorldOrchestration.HelloWorldOrchestrationInput { HelloText = "SelfHostServiceClientTestInputArg" }).Result;

            microservices.Add(svc);

            host.WaitOnInstances(host, microservices);
        }


        [TestMethod]
        public void SelfHostServiceClientTestWithHubName()
        {
            var loggerFact = UnitTestLogging.GetDebugLoggerFactory();

            ServiceClient client = ClientHelperExtensions.CreateMicroserviceClient(ServiceBusConnectionString, StorageConnectionString, nameof(SelfHostServiceClientTest));

            var orchestrationInput = new HelloWorldOrchestration.HelloWorldOrchestrationInput
            {
                HelloText = "SelfHostServiceClientTestInputArg",
                Context = new Dictionary<string, object> { { "ActivityId", "SelfHostServiceClientTestWithHubName" } }
            };

            var svc = client.StartServiceAsync(cSvcName, orchestrationInput).Result;

            Assert.IsTrue(svc != null);
        }
    }
}
