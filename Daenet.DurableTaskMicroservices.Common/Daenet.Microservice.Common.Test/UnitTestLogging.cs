using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.Extensions;
using Daenet.DurableTaskMicroservices.Host;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Daenet.Microservice.Common.Test
{
    [TestClass]
    public class UnitTestLogging
    {
        private static string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString;
        private static string StorageConnectionString = ConfigurationManager.ConnectionStrings["Storage"].ConnectionString;

        [TestMethod]
        public void SelfHostWithLoggingTest()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug();

            List<OrchestrationState> runningInstances;

            ServiceHost host = HostHelpersExtensions.CreateMicroserviceHost(ServiceBusConnectionString, StorageConnectionString, nameof(SelfHostWithLoggingTest), true, out runningInstances);

            var microservices = host.StartServiceHostAsync(Path.Combine(), runningInstances: runningInstances).Result;

            host.WaitOnInstances(host, microservices);

        }

      
    }
}
