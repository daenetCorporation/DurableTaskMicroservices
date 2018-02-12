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
using Daenet.DurableTask.SqlStateProvider;
using DurableTask.Core;
using DurableTask.ServiceBus;
using DurableTask.ServiceBus.Tracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Daenet.DurableTaskMicroservices.Host
{
  
    /// <summary>
    /// This class is currentlly the only one in this project.
    /// It is require, because if references to DurableTask.Servicebus, which in NOT .NET Core component.
    /// Once microsoft makes it .NET core, we will move this code to Daenet.DurableTaskMicroservices project.
    /// </summary>
    public static class HostHelpersExtensions
    {
        public static ServiceHost CreateMicroserviceHost(string serviceBusConnectionString, string storageConnectionString, string hubName,
           bool recreateHubAndStore, out List<OrchestrationState> runningInstances, ILoggerFactory loggerFactory = null, bool useSqlInstanceStore = false)
        {

            IOrchestrationServiceInstanceStore instanceStore;

            if (!useSqlInstanceStore)
            {
                instanceStore = new AzureTableInstanceStore(hubName, storageConnectionString);
            }
            else
            {
                instanceStore = new SqlInstanceStore(hubName, storageConnectionString);
            }

            ServiceBusOrchestrationService orchestrationServiceAndClient =
                 new ServiceBusOrchestrationService(serviceBusConnectionString, hubName, instanceStore, null, null);

            ServiceHost host;

            host = new ServiceHost(orchestrationServiceAndClient, orchestrationServiceAndClient, instanceStore, recreateHubAndStore, loggerFactory);

            try
            {
                //if (purgeStore)
                //{
                //    instanceStore.InitializeStoreAsync(false).Wait();
                //    instanceStore.PurgeOrchestrationHistoryEventsAsync(DateTime.Now.AddYears(1), OrchestrationStateTimeRangeFilterType.OrchestrationCreatedTimeFilter).Wait();
                //}

                // Not available on interface yet.
                if (instanceStore is AzureTableInstanceStore)
                    runningInstances = ((AzureTableInstanceStore)instanceStore).GetRunningInstances();
                else
                    runningInstances = ((SqlInstanceStore)instanceStore).GetRunningInstances();
            }
            catch (Exception)
            {
                runningInstances = new List<OrchestrationState>();
                // This will fail if the store is not created already
            }

            return host;
        }

        public static List<OrchestrationState> GetRunningInstances(this SqlInstanceStore instanceStore)
        {
            List<OrchestrationState> instances = new List<OrchestrationState>();

            var byNameQuery = new OrchestrationStateQuery();
            byNameQuery.AddStatusFilter(OrchestrationStatus.Running);

            instances.AddRange(GetInstancesByState(instanceStore, OrchestrationStatus.Running));

           // instances.AddRange(GetInstancesByState(instanceStore, OrchestrationStatus.ContinuedAsNew));

            instances.AddRange(GetInstancesByState(instanceStore, OrchestrationStatus.Pending));

            return instances;
        }

        public static IEnumerable<OrchestrationState> GetInstancesByState(this SqlInstanceStore instanceStore, OrchestrationStatus status)
        {
            List<OrchestrationState> instances = new List<OrchestrationState>();

            var byNameQuery = new OrchestrationStateQuery();
            byNameQuery.AddStatusFilter(status);

            return instanceStore.QueryOrchestrationStatesAsync(byNameQuery).Result;
        }

        public static List<OrchestrationState> GetRunningInstances(this AzureTableInstanceStore instanceStore)
        {
            List<OrchestrationState> instances = new List<OrchestrationState>();

            var byNameQuery = new OrchestrationStateQuery();
            byNameQuery.AddStatusFilter(OrchestrationStatus.Running);

            instances.AddRange(GetInstancesByState(instanceStore, OrchestrationStatus.Running));

            instances.AddRange(GetInstancesByState(instanceStore, OrchestrationStatus.ContinuedAsNew));

            instances.AddRange(GetInstancesByState(instanceStore, OrchestrationStatus.Pending));

            return instances;
        }

        public static IEnumerable<OrchestrationState> GetInstancesByState(this AzureTableInstanceStore instanceStore, OrchestrationStatus status)
        {
            List<OrchestrationState> instances = new List<OrchestrationState>();

            var byNameQuery = new OrchestrationStateQuery();
            byNameQuery.AddStatusFilter(status);

            return instanceStore.QueryOrchestrationStatesAsync(byNameQuery).Result;
        }

    }
}
