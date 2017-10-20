using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;

namespace Daenet.Microservice.Common.Test
{
    [TestClass]
    public class UnitTestLogging
    {
        [TestMethod]
        public void SelfHostWithLoggingTest()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug();

            Daenet.DurableTaskMicroservices.Host.Host host = new Daenet.DurableTaskMicroservices.Host.Host(loggerFactory);

            host.StartServiceHost(Path.Combine());

            Thread.Sleep(int.MaxValue);
        }
    }
}
