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
using Daenet.DurableTask.SqlStateProvider;
using DurableTask;
using DurableTask.Core;
using DurableTask.ServiceBus;
using DurableTask.ServiceBus.Tracking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Daenet.DurableTaskMicroservices.Tests
{
    // AppContext.SetSwitch("Switch.System.IdentityModel.DisableMul‌​tipleDNSEntriesInSAN‌​Certificate", true);
    // See: https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/mitigation-x509certificateclaimset-findclaims-method

    [TestClass]
    public class UnitTests
    {
        private static string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["ServiceBus"]?.ConnectionString;
        private static string StorageConnectionString = ConfigurationManager.ConnectionStrings["Storage"]?.ConnectionString;
        private static string SqlStorageConnectionString = ConfigurationManager.ConnectionStrings["SqlStorage"]?.ConnectionString;

        private static ServiceHost createMicroserviceHost(IOrchestrationServiceInstanceStore instanceStore)
        {
             ServiceBusOrchestrationService orchestrationServiceAndClient =
               new ServiceBusOrchestrationService(ServiceBusConnectionString, "UnitTestTmp", instanceStore, null, null);

            //instanceStore.PurgeOrchestrationHistoryEventsAsync(DateTime.Now.AddYears(1), OrchestrationStateTimeRangeFilterType.OrchestrationCreatedTimeFilter).Wait();
            ServiceHost host;

            host = new ServiceHost(orchestrationServiceAndClient, orchestrationServiceAndClient, instanceStore, false);

            return host;
        }

        private static ServiceHost getMicroserviceHostWithTableStorage()
        {
            IOrchestrationServiceInstanceStore instanceStore = new AzureTableInstanceStore("UnitTestTmp", StorageConnectionString);
            return createMicroserviceHost(instanceStore);
        }


        private static ServiceHost getMicroServiceWithSqlInstanceStoreHost()
        {
            IOrchestrationServiceInstanceStore instanceStore = new SqlInstanceStore("Dtf", SqlStorageConnectionString);
            //instanceStore.DeleteStoreAsync().Wait();

            return createMicroserviceHost(instanceStore);
        }


        [TestMethod]
        public void OpenAndStartServiceHostTestWithSB()
        {
            var host = getMicroserviceHostWithTableStorage();

            Microservice service = new Microservice();
            service.InputArgument = new TestOrchestrationInput()
            {
                Counter = 2,
                Delay = 1000,
            };

            service.OrchestrationQName = typeof(CounterOrchestration).AssemblyQualifiedName;

            service.ActivityQNames = new string[]{
                typeof(Task1).AssemblyQualifiedName,  typeof(Task2).AssemblyQualifiedName,
            };

            host.LoadService(service);

            host.OpenAsync().Wait();

            // This is client side code.
            var instance = host.StartServiceAsync(service.OrchestrationQName, service.InputArgument).Result;

            Debug.WriteLine($"Microservice instance {instance.OrchestrationInstance.InstanceId} started");

            host.WaitOnInstanceAsync(instance).Wait();
        }


        /// <summary>
        /// Runs the orchestration with SQL Instance store.
        /// </summary>
        [TestMethod]
        public void OpenAndStartServiceHostTestWithSql()
        {
            var host = getMicroServiceWithSqlInstanceStoreHost();

            Microservice service = new Microservice();
            service.InputArgument = new TestOrchestrationInput()
            {
                Counter = 2,
                Delay = 1000,
            };

            service.OrchestrationQName = typeof(CounterOrchestration).AssemblyQualifiedName;

            service.ActivityQNames = new string[]{
                typeof(Task1).AssemblyQualifiedName,  typeof(Task2).AssemblyQualifiedName,
            };

            host.LoadService(service);

            host.OpenAsync().Wait();

            // This is client side code.
            var instance = host.StartServiceAsync(service.OrchestrationQName, service.InputArgument).Result;

            Debug.WriteLine($"Microservice instance {instance.OrchestrationInstance.InstanceId} started");

            host.WaitOnInstanceAsync(instance).Wait();
        }


        [TestMethod]
        [DataRow("CounterOrchestration.config.xml")]
        public void RunServiceFromXml(string fileName)
        {
            var host = getMicroserviceHostWithTableStorage();

            Microservice microSvc = host.LoadServiceFromXml(UtilsTests.GetPathForFile(fileName), 
                new List<Type>(){ typeof(TestOrchestrationInput) });

            host.OpenAsync().Wait();

            var instance = host.StartServiceAsync(microSvc.OrchestrationQName, microSvc.InputArgument).Result;

            Debug.WriteLine($"Microservice instance {instance.OrchestrationInstance.InstanceId} started");

            host.WaitOnInstanceAsync(instance).Wait();
        }
        
        
    }
}
