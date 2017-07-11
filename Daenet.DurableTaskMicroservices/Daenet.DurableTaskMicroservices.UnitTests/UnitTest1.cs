using Daenet.DurableTask.Microservices;
using DurableTask;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Daenet.DurableTaskMicroservices.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private static TaskHubClient m_HubClient;
        private static TaskHubWorker m_TaskHubWorker;

        private static string ServiceBusConnectionString = "Endpoint=sb://homagmdw.servicebus.windows.net/;SharedAccessKeyName=DTF;SharedAccessKey=SDcRR1HGh5Bi+XPqJTFOwj6Rb4nx9f8XA/qLUYgRP00=";
        private static string StorageConnectionString = "";


        private static ServiceHost createMicroserviceHost()
        {
            ServiceHost host;

            host = new ServiceHost(ServiceBusConnectionString, StorageConnectionString, "UnitTestHub");

            return host;
        }

        [TestMethod]
        public void TestMethod1()
        {
            var host = createMicroserviceHost();

            Microservice service = new Microservice();
            service.ServiceConfiguration = new TestOrchestrationInput()
            {
                Counter = 5,
                Delay = 4000,
            };

            service.OrchestrationQName = typeof(CounterOrchestration).AssemblyQualifiedName;

            service.ActivityQNames = new string[]{
                typeof(Task1).AssemblyQualifiedName,  typeof(Task2).AssemblyQualifiedName,
            };

            host.LoadService(service);

            host.Open();

            host.StartService(service.OrchestrationQName, service.ServiceConfiguration);

            ManualResetEvent mEvent = new ManualResetEvent(false);

            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(1000);

                        var cnt = host.GetNumOfRunningInstances(service);

                        Console.WriteLine(String.Format("Child:{0} - Main:{1}", service, cnt));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        // mEvent.Set();
                        //return;
                    }
                }
            }).Start();

            mEvent.WaitOne();
        }
    }
}
