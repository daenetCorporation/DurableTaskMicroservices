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
using DurableTask.ServiceBus;
using DurableTask.ServiceBus.Tracking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Daenet.DurableTaskMicroservices.UnitTests
{
    // AppContext.SetSwitch("Switch.System.IdentityModel.DisableMul‌​tipleDNSEntriesInSAN‌​Certificate", true);
    // See: https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/mitigation-x509certificateclaimset-findclaims-method

    [TestClass]
    public class UnitTests
    {
        private static string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString;
        private static string StorageConnectionString = ConfigurationManager.ConnectionStrings["Storage"].ConnectionString;

        private static ServiceHost createMicroserviceHost()
        {
            IOrchestrationServiceInstanceStore instanceStore = new AzureTableInstanceStore("UnitTestTmp", StorageConnectionString);
            ServiceBusOrchestrationService orchestrationServiceAndClient =
               new ServiceBusOrchestrationService(ServiceBusConnectionString, "UnitTestTmp", instanceStore, null, null);

            instanceStore.PurgeOrchestrationHistoryEventsAsync(DateTime.Now.AddYears(1), OrchestrationStateTimeRangeFilterType.OrchestrationCreatedTimeFilter).Wait();
            ServiceHost host;

            host = new ServiceHost(orchestrationServiceAndClient, orchestrationServiceAndClient, instanceStore, false);

            return host;
        }

        [TestMethod]
        public void OpenAndStartServiceHostTest()
        {
            var host = createMicroserviceHost();

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
        [DataRow("CounterOrchestration.xml")]
        public void LoadServiceFromXml(string fileName)
        {
            Microservice microSvc;

            var host = createMicroserviceHost();
       
            var instances = host.LoadServiceFromXml(UtilsTests.GetPathForFile(fileName), 
                new List<Type>(){ typeof(TestOrchestrationInput) }, out microSvc);

            var instance = host.StartServiceAsync(microSvc.OrchestrationQName, microSvc.InputArgument).Result;

            Debug.WriteLine($"Microservice instance {instance.OrchestrationInstance.InstanceId} started");

            waitOnInstance(host, microSvc, instance);
        }

        private void waitOnInstance(ServiceHost host, Microservice service, MicroserviceInstance instance)
        {
            ManualResetEvent mEvent = new ManualResetEvent(false);

            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(100);

                        var cnt = host.GetNumOfRunningInstancesAsync(service).Result;

                        if (cnt == 10)
                        {
                            mEvent.Set();
                            Debug.WriteLine($"Microservice instance {instance.OrchestrationInstance.InstanceId} completed.");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        mEvent.Set();
                        Assert.Fail(ex.Message);
                    }
                }
            }).Start();

            mEvent.WaitOne();
        }


        [TestMethod]
        [DataRow("CounterOrchestration.xml")]
        public void LoadFromConfigTest(string fileName)
        {
            var service = UtilsTests.DeserializeService(UtilsTests.GetPathForFile(fileName));

            var host = createMicroserviceHost();

            host.LoadService(service);

            host.OpenAsync().Wait();

            var instance = host.StartServiceAsync(service.OrchestrationQName, service.InputArgument).Result;

            Debug.WriteLine($"Microservice instance {instance.OrchestrationInstance.InstanceId} started");

            waitOnInstance(host, service, instance);
        }
    }
}
