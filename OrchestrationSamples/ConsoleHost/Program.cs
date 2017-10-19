using CounterOrchestration;
using Daenet.DurableTask.Microservices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleHost
{
    class Program
    {
        private static string m_ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["serviceBus"].ConnectionString;

        private static string m_StorageConnectionString = ConfigurationManager.ConnectionStrings["storage"].ConnectionString;

        static void Main(string[] args)
        {
            runSample1();
        }
        

        private static void runSample1()
        {
            ServiceHost host = new ServiceHost(m_ServiceBusConnectionString, m_StorageConnectionString, nameof(runSample1));

            Microservice microSvc= new Microservice();

            #region How to create Config automatically?
            //microSvc.InputArgument = new CounterOrchestrationInput() { Counter = 1, Delay = 2 };

            //xmlSerializeService(microSvc, "c:\\temp\\aaa.xml");
            #endregion

            var instances = host.LoadServiceFromXml("counterorchestration.config.xml", 
                new List<Type>() { typeof(CounterOrchestrationInput)}, out microSvc);

            host.Open();

            var instance = host.StartService(microSvc.OrchestrationQName, microSvc.InputArgument);

            Debug.WriteLine($"Microservice instance {instance.OrchestrationInstance.InstanceId} started");

            waitOnInstance(host, microSvc, instance);
        }

     
        /// <summary>
        /// This method is waiting on the single instance to be completed. 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="service"></param>
        /// <param name="instance"></param>
        private static void waitOnInstance(ServiceHost host, Microservice service, MicroserviceInstance instance)
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
                    }
                }
            }).Start();


            mEvent.WaitOne();
        }


        private static void xmlSerializeService(Microservice svc, string fileName)
        {
            using (XmlWriter writer = XmlWriter.Create(fileName, new XmlWriterSettings() { Indent = true }))
            {
                DataContractSerializerSettings sett = new DataContractSerializerSettings();
                DataContractSerializer ser = new DataContractSerializer(typeof(Microservice),
                    new List<Type>() { typeof(CounterOrchestrationInput) });
                ser.WriteObject(writer, (Microservice)svc);
            }
        }

    }
}
