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
    public class ServiceHostTests
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

            Daenet.DurableTaskMicroservices.Host.Host host2 = new Daenet.DurableTaskMicroservices.Host.Host();

            host2.StartServiceHost();

            Thread.Sleep(int.MaxValue);
        }

        
    }
}
