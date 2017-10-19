using Daenet.DurableTask.Microservices;
using DurableTask;
using Microsoft.Extensions.Logging;
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
    public class ServiceHostTests
    {
        private static string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString;
        private static string StorageConnectionString = ConfigurationManager.ConnectionStrings["Storage"].ConnectionString;

        private static ServiceHost createMicroserviceHost()
        {
            ServiceHost host;

            host = new ServiceHost(ServiceBusConnectionString, StorageConnectionString, "UnitTestHub");

            return host;
        }

        [TestMethod]
        public void SelfHostTest()
        {
            Daenet.DurableTaskMicroservices.Host.Host host2 = new Daenet.DurableTaskMicroservices.Host.Host();

            host2.StartServiceHost(Path.Combine(AppContext.BaseDirectory, "TestConfiguration"));

            Thread.Sleep(int.MaxValue); //TODO
        }

        [TestMethod]
        public void SelfHostWithLoggingTest()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug();
        
            Daenet.DurableTaskMicroservices.Host.Host host2 = new Daenet.DurableTaskMicroservices.Host.Host(loggerFactory);

            host2.StartServiceHost(Path.Combine(AppContext.BaseDirectory, "TestConfiguration"));

            Thread.Sleep(int.MaxValue); //TODO
        }

    }
}
