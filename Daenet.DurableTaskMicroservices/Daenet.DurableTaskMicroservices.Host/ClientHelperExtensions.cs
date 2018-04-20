using Daenet.DurableTask.Microservices;
using Daenet.DurableTask.SqlStateProvider;
using DurableTask.Core;
using DurableTask.ServiceBus;
using DurableTask.ServiceBus.Tracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daenet.DurableTaskMicroservices.Host
{
    public static class ClientHelperExtensions
    {
        public static ServiceClient CreateMicroserviceClient(string serviceBusConnectionString, string storageConnectionString, string hubName, ILogger logger = null)
        {
            IOrchestrationServiceInstanceStore instanceStore;

            if (storageConnectionString.ToLower().Contains("server="))
            {
                instanceStore = new SqlInstanceStore(hubName, storageConnectionString);
            }
            else
            {
                instanceStore = new AzureTableInstanceStore(hubName, storageConnectionString);
            }

            ServiceBusOrchestrationService orchestrationServiceAndClient =
                 new ServiceBusOrchestrationService(serviceBusConnectionString, hubName, instanceStore, null, null);

            ServiceClient client;

            client = new ServiceClient(orchestrationServiceAndClient);

            return client;
        }
    }
}
