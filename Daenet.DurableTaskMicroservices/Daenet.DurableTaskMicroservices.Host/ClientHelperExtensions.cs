using Daenet.DurableTask.Microservices;
using Daenet.DurableTask.SqlInstanceStoreProvider;
using Daenet.DurableTaskMicroservices.Core;
using DurableTask.Core;
using DurableTask.ServiceBus;
using DurableTask.ServiceBus.Tracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Host
{
    /// <summary>
    /// Contains client related methods, which have dependency to old full .NET DurableTask.ServiceBus.
    /// Once microsoft makes it .NET core, we will move this code to Daenet.DurableTaskMicroservices project.
    /// </summary>
    public static class ClientHelperExtensions
    {
        public static ServiceClient CreateMicroserviceClient(string serviceBusConnectionString, string storeConnectionString, string hubName, ILogger logger = null, bool purgeStore = false)
        {
            IOrchestrationServiceInstanceStore instanceStore =
                InstanceStoreFactory.CreateInstanceStore(hubName, storeConnectionString, purgeStore);

            ServiceBusOrchestrationService orchestrationServiceAndClient =
                 new ServiceBusOrchestrationService(serviceBusConnectionString, hubName, instanceStore, null, null);

            ServiceClient client;

            client = new ServiceClient(orchestrationServiceAndClient);

            return client;
        }


        public static async Task<IEnumerable<OrchestrationState>> GetInstancesByState(this IOrchestrationServiceInstanceStore instanceStore, OrchestrationStatus status)
        {
            List<OrchestrationState> instances = new List<OrchestrationState>();

            IEnumerable<OrchestrationState> states;
            
            var query = new OrchestrationStateQuery();
            query.AddStatusFilter(status);

            if (instanceStore is AzureTableInstanceStore)
                states = await ((AzureTableInstanceStore)instanceStore).QueryOrchestrationStatesAsync(query);
            else
                states = await ((SqlInstanceStore)instanceStore).QueryOrchestrationStatesAsync(query);

            return states;
        }
    }
}
