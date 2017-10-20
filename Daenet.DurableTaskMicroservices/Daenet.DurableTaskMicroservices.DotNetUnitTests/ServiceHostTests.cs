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
        private static string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["ServiceBus"]?.ConnectionString;
        private static string StorageConnectionString = ConfigurationManager.ConnectionStrings["Storage"]?.ConnectionString;

        private static ServiceHost createMicroserviceHost()
        {
            ServiceHost host;

            host = new ServiceHost(ServiceBusConnectionString, StorageConnectionString, "UnitTestHub");

            return host;
        }

        [TestMethod]
        public void SelfHostTest()
        {
            Daenet.DurableTaskMicroservices.Host.Host host = new Daenet.DurableTaskMicroservices.Host.Host();

            host.StartServiceHost(Path.Combine(AppContext.BaseDirectory, "TestConfiguration"));

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
