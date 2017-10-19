using Daenet.DurableTask.Microservices;
using DurableTask;
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
        private static string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["serviceBus"].ConnectionString;
        private static string StorageConnectionString = ConfigurationManager.ConnectionStrings["storage"].ConnectionString;

        private static ServiceHost createMicroserviceHost()
        {
            ServiceHost host;

            host = new ServiceHost(ServiceBusConnectionString, StorageConnectionString, "UnitTestHub");

            return host;
        }

        [TestMethod]
        public void OpenAndStartServiceHostTest()
        {
            var host = createMicroserviceHost();

            Microservice service = new Microservice();
            service.InputArgument = new TestOrchestrationInput()
            {
                Counter = 3,
                Delay = 1000,
            };

            service.OrchestrationQName = typeof(CounterOrchestration).AssemblyQualifiedName;

            service.ActivityQNames = new string[]{
                typeof(Task1).AssemblyQualifiedName,  typeof(Task2).AssemblyQualifiedName,
            };

            host.LoadService(service);

            host.Open();

            // This is client side code.
            var instance = host.StartService(service.OrchestrationQName, service.InputArgument);

            Debug.WriteLine($"Microservice instance {instance.OrchestrationInstance.InstanceId} started");

            waitOnInstance(host, service, instance);
        }


        [TestMethod]
        [DataRow("CounterOrchestration.xml")]
        public void LoadServiceFromXml(string fileName)
        {
            Microservice microSvc;

            var host = createMicroserviceHost();
       
            var instances = host.LoadServiceFromXml(UtilsTests.GetPathForFile(fileName), 
                new List<Type>(){ typeof(TestOrchestrationInput) }, out microSvc);

            var instance = host.StartService(microSvc.OrchestrationQName, microSvc.InputArgument);

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
                        Thread.Sleep(1000);

                        var cnt = host.GetNumOfRunningInstances(service);

                        if (cnt == 0)
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

            host.Open();

            var instance = host.StartService(service.OrchestrationQName, service.InputArgument);

            Debug.WriteLine($"Microservice instance {instance.OrchestrationInstance.InstanceId} started");

            waitOnInstance(host, service, instance);
        }
    }
}
