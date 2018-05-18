using Daenet.DurableTaskMicroservices.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daenet.DurableTask.Microservices;
using DurableTask.Core;

namespace GetTweetsMention
{
    class Program
    {
        private static string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString;
        private static string StorageConnectionString = ConfigurationManager.ConnectionStrings["Storage"].ConnectionString;
        private static string SqlConnectionString = ConfigurationManager.ConnectionStrings["Sql"].ConnectionString;



        private static ILoggerFactory getLoggerFactory()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.Trace);
            return loggerFactory;
        }
        static void Main(string[] args)
        {
            var loggerFact = getLoggerFactory();

            List<OrchestrationState> runningInstances;

            ServiceHost host = HostHelpersExtensions.CreateMicroserviceHost(ServiceBusConnectionString, SqlConnectionString, "TweetUsingSqlTest", true, out runningInstances, loggerFact, true);

            var microservices = host.StartServiceHostAsync(Path.Combine(), runningInstances: runningInstances, context: new Dictionary<string, object>() { { "company", "daenet" } }).Result;

            host.WaitOnInstances(host, microservices);
        }
    }
}
