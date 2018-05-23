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
using Daenet.DurableTaskMicroservices.Common.Extensions;
using Daenet.DurableTaskMicroservices.Host;
using DurableTask.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Daenet.Common.Logging.Sql;
using Daenet.DurableTaskMicroservices.Core;

namespace Daenet.DurableTaskMicroservices.Common.Test
{
    [TestClass]
    public class UnitTestLogging
    {
        internal static string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString;
        internal static string StorageConnectionString = ConfigurationManager.ConnectionStrings["Storage"].ConnectionString;
        internal static string SqlStorageConnectionString = ConfigurationManager.ConnectionStrings["SqlStorage"].ConnectionString;


        internal static ILoggerFactory GetDebugLoggerFactory(LogLevel level = LogLevel.Trace)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug(level);

            return loggerFactory;
        }

        private static ILoggerFactory getSqlLoggerFactory()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("sqlloggersettings.json");
            var Configuration = builder.Build();

            string sectionName = "Logging";
            var cfg = Configuration.GetSection(sectionName);

            ILoggerFactory loggerFactory = new LoggerFactory().AddSqlServerLogger(cfg);
            return loggerFactory;
        }


        [TestMethod]
        public void SelfHostWithLoggingTest()
        {
            //ServiceEventReceiver eventReceiver = new ServiceEventReceiver(EventLevel.LogAlways);

            //ObservableEventListener eventListener = new ObservableEventListener();
            //eventListener.Subscribe(new TraceEventReceiver());
            //eventListener.EnableEvents(DefaultEventSource.Log, EventLevel.LogAlways);

            var loggerFact = GetDebugLoggerFactory();

            List<OrchestrationState> runningInstances;

            ServiceHost host = HostHelpersExtensions.CreateMicroserviceHost(ServiceBusConnectionString, StorageConnectionString, nameof(SelfHostWithLoggingTest), true, out runningInstances, loggerFact);

            var microservices = host.StartServiceHostAsync(Path.Combine(), runningInstances: runningInstances, context: new Dictionary<string, object>() { { "company", "daenet" } }).Result;

            host.WaitOnInstances(host, microservices);
        }


        [TestMethod]
        public void SelfHostWithSqlLoggingTest()
        {
            var loggerFact = getSqlLoggerFactory();

            List<OrchestrationState> runningInstances;

            ServiceHost host = HostHelpersExtensions.CreateMicroserviceHost(ServiceBusConnectionString, SqlStorageConnectionString, nameof(SelfHostWithSqlLoggingTest), false, out runningInstances, loggerFact);

            var microservices = host.StartServiceHostAsync(Path.Combine(), runningInstances: runningInstances, context: new Dictionary<string, object>() { { "company", "daenet" } }).Result;

            host.WaitOnInstances(host, microservices);
        }

        [TestMethod]
        public void SelfHostServiceClientTest()
        {
            var loggerFact = getSqlLoggerFactory();

            List<OrchestrationState> runningInstances;

            ServiceHost host = HostHelpersExtensions.CreateMicroserviceHost(ServiceBusConnectionString, SqlStorageConnectionString, nameof(SelfHostServiceClientTest), true, out runningInstances, loggerFact);

            var microservices = host.StartServiceHostAsync(Path.Combine(), runningInstances: runningInstances, context: new Dictionary<string, object>() { { "company", "daenet" } }).Result;

            ServiceClient client = ClientHelperExtensions.CreateMicroserviceClient(ServiceBusConnectionString, SqlStorageConnectionString, nameof(SelfHostServiceClientTest));

            string svcName = "Daenet.Microservice.Common.Test.HelloWorldOrchestration.HelloWorldOrchestration";

            var svc = client.StartServiceAsync(svcName, new HelloWorldOrchestration.HelloWorldOrchestrationInput { HelloText = "SelfHostServiceClientTestInputArg" }).Result;
            microservices.Add(svc);

            host.WaitOnInstances(host, microservices);
        }     
    }
}
