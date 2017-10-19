using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Daenet.DurableTask.Microservices;
using System.Xml;
using System.Runtime.Serialization;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Daenet.DurableTaskMicroservices.UnitTests;

namespace Daenet.DurableTaskMicroservices.UnitTests
{
    [TestClass]
    public class UtilsTests
    {
        [TestMethod]
        [DataRow("CounterOrchestrationSvc.xml")]
        public void SerializeXmlConfigTest(string fileName)
        {
            Microservice service = getMicroService();

            xmlSerializeService(service, GetPathForFile(fileName));
        }

        [TestMethod]
        [DataRow("CounterOrchestrationSvc.json")]
        public void SerializeJsonConfigTest(string fileName)
        {
            Microservice service = getMicroService();

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(service);

            File.WriteAllText(GetPathForFile(fileName), jsonString);
        }

        private static Microservice getMicroService()
        {
            return new Microservice()
            {
                InputArgument = new TestOrchestrationInput()
                {
                    Counter = 3,
                    Delay = 1000,
                },

                OrchestrationQName = typeof(CounterOrchestration).AssemblyQualifiedName,

                ActivityQNames = new string[]
                {
                    typeof(Task1).AssemblyQualifiedName,
                    typeof(Task2).AssemblyQualifiedName,
                },
            };
        }

        private static void xmlSerializeService(Microservice svc, string fileName)
        {
            using (XmlWriter writer = XmlWriter.Create(fileName, new XmlWriterSettings() { Indent = true }))
            {
                DataContractSerializerSettings sett = new DataContractSerializerSettings();
                DataContractSerializer ser = new DataContractSerializer(typeof(Microservice),
                    loadKnownTypes());
                ser.WriteObject(writer, (Microservice)svc);
            }
        }

        internal static Microservice DeserializeService(string configFile)
        {
            using (XmlReader writer = XmlReader.Create(configFile))
            {
                DataContractSerializerSettings sett = new DataContractSerializerSettings();
                DataContractSerializer ser = new DataContractSerializer(typeof(Microservice), loadKnownTypes());
                object svc = ser.ReadObject(writer);
                return svc as Microservice;
            }
        }

        private static Type[] loadKnownTypes()
        {
            List<Type> types = new List<Type>();

            string[] patterns = new string[] { "*.dll", "*.exe" };

            foreach (var pattern in patterns)
            {
                Assembly asm = typeof(UtilsTests).Assembly;

                foreach (var type in asm.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(DataContractAttribute)).Count() > 0)
                    {
                        types.Add(type);
                        Trace.WriteLine(type);
                    }
                }
            }

            return types.ToArray();
        }

        internal static string GetPathForFile(string fileName)
        {
            return Path.Combine(Environment.CurrentDirectory, $"TestConfiguration\\{fileName}");
        }
    }
}
