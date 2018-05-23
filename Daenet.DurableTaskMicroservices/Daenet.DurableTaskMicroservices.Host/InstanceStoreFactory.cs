
using Daenet.DurableTask.SqlInstanceStoreProvider;
using DurableTask.Core;
using DurableTask.ServiceBus.Tracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daenet.DurableTaskMicroservices.Host
{
    internal static class InstanceStoreFactory
    {
        public static IOrchestrationServiceInstanceStore CreateInstanceStore(string hubName, string storageConnectionString, bool purgeStore = false)
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

            if (purgeStore)
            {
                instanceStore.InitializeStoreAsync(false).Wait();
                instanceStore.PurgeOrchestrationHistoryEventsAsync(DateTime.Now.AddYears(-1), OrchestrationStateTimeRangeFilterType.OrchestrationCreatedTimeFilter).Wait();
            }

            return instanceStore;
        }
    }
}
