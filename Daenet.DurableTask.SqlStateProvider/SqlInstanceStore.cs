using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DurableTask.Core.History;
using Newtonsoft.Json;
using System.Globalization;
using System.Data.SqlTypes;
using DurableTask.Core;
using DurableTask.Core.Tracking;
using System.Data.SqlClient;
using System.Linq;

namespace Daenet.DurableTask.SqlStateProvider
{
    public class SqlInstanceStore : IOrchestrationServiceInstanceStore
    {
        public SqlClient Client { get; set; }

        public int MaxHistoryEntryLength => throw new NotImplementedException();

        public SqlInstanceStore(string baseTableName, string sqlConnectionString, string schemaName = null)
        {
            Client = new SqlClient(baseTableName, sqlConnectionString, schemaName);
        }

        #region Public Methods

        public async Task InitializeStoreAsync(bool recreate)
        {
            if (recreate)
            {
                await DeleteStoreAsync();
            }

            await Client.CreateStoreIfNotExistsAsync();
        }

        public async Task DeleteStoreAsync()
        {
            await Client.DeleteStoreIfExistsAsync();
        }

        public async Task<object> WriteEntitiesAsync(IEnumerable<InstanceEntityBase> entities)
        {
            var workItems = entities.OfType<OrchestrationWorkItemInstanceEntity>();

            await Client.WriteWorkItemAsync(workItems);

            var stateItems = entities.OfType<OrchestrationStateInstanceEntity>();

            await Client.WriteStateAsync(stateItems);

            return null;
        }

        /// <summary>
        /// Get OrchestrationState
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="executionId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<OrchestrationStateInstanceEntity>> GetEntitiesAsync(string instanceId, string executionId)
        {
            var results = await Client.ReadOrchestrationStateAsync(instanceId, executionId);

            return results;
        }

        public async Task<object> DeleteEntitiesAsync(IEnumerable<InstanceEntityBase> entities)
        {
            var workItems = entities.OfType<OrchestrationWorkItemInstanceEntity>();

            await Client.DeleteWorkItemsAsync(workItems);

            var stateItems = entities.OfType<OrchestrationStateInstanceEntity>();

            await Client.DeleteStatesAsync(stateItems);

            return null;
        }

        public async Task<IEnumerable<OrchestrationStateInstanceEntity>> GetOrchestrationStateAsync(string instanceId, bool allInstances)
        {
            var query = new OrchestrationStateQuery().AddInstanceFilter(instanceId);

            var stateEntities = (await Client.QueryOrchestrationStatesAsync(query))?.Results;

            IEnumerable<OrchestrationStateInstanceEntity> jumpStartEntities = await Client.QueryJumpStartOrchestrationsAsync(query);

            IEnumerable<OrchestrationState> states = stateEntities.Select(stateEntity => stateEntity);
            IEnumerable<OrchestrationState> jumpStartStates = jumpStartEntities.Select(j => j.State)
                .Where(js => !states.Any(s => s.OrchestrationInstance.InstanceId == js.OrchestrationInstance.InstanceId));

            var newStates = states.Concat(jumpStartStates);

            if (allInstances)
            {
                return newStates.Select(tableStateToStateEvent); ;
            }

            foreach (OrchestrationState state in newStates)
            {
                // TODO: This will just return the first non-ContinuedAsNew orchestration and not the latest one.
                if (state.OrchestrationStatus != OrchestrationStatus.ContinuedAsNew)
                {
                    return new List<OrchestrationStateInstanceEntity>() { tableStateToStateEvent(state) };
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the orchestration state for a given instance and execution id
        /// </summary>
        /// <param name="instanceId">The instance id to return state for</param>
        /// <param name="executionId">The execution id to return state for</param>
        /// <returns>The matching orchestation state or null if not found</returns>
        public async Task<OrchestrationStateInstanceEntity> GetOrchestrationStateAsync(string instanceId, string executionId)
        {
            var result = (await Client.ReadOrchestrationStateAsync(instanceId, executionId)).FirstOrDefault();

            if (result == null)
            {
                // Query from JumpStart table
                result = (await Client.QueryJumpStartOrchestrationsAsync(
                       new OrchestrationStateQuery()
                        .AddInstanceFilter(instanceId, executionId))
                        .ConfigureAwait(false))
                    .FirstOrDefault();
            }

            return (result != null) ? tableStateToStateEvent(result.State) : null;
        }

        /// <summary>
        /// Gets the list of history events for a given instance and execution id
        /// </summary>
        /// <param name="instanceId">The instance id to return history for</param>
        /// <param name="executionId">The execution id to return history for</param>
        /// <returns>List of history events</returns>
        public async Task<IEnumerable<OrchestrationWorkItemInstanceEntity>> GetOrchestrationHistoryEventsAsync(string instanceId, string executionId)
        {
            IEnumerable<OrchestrationWorkItemInstanceEntity> entities = await Client.ReadWorkItemsAsync(instanceId, executionId);
            return entities.OrderBy(ee => ee.SequenceNumber);
        }

        /// <summary>
        /// Purges history from storage for given time range
        /// </summary>
        /// <param name="thresholdDateTimeUtc">The datetime in UTC to use as the threshold for purging history</param>
        /// <param name="timeRangeFilterType">What to compare the threshold date time against</param>
        /// <returns>The number of history events purged.</returns>
        public async Task<int> PurgeOrchestrationHistoryEventsAsync(DateTime thresholdDateTimeUtc, OrchestrationStateTimeRangeFilterType timeRangeFilterType)
        {
            var purgeCount = await Client.PurgeOrchestrationInstanceHistoryAsync(thresholdDateTimeUtc, timeRangeFilterType);

            return purgeCount;
        }


        public Task<object> WriteJumpStartEntitiesAsync(IEnumerable<OrchestrationJumpStartInstanceEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task<object> DeleteJumpStartEntitiesAsync(IEnumerable<OrchestrationJumpStartInstanceEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrchestrationJumpStartInstanceEntity>> GetJumpStartEntitiesAsync(int top)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Private Methods

        private OrchestrationStateInstanceEntity tableStateToStateEvent(OrchestrationState state)
        {
            return new OrchestrationStateInstanceEntity { State = state };
        }

        #endregion

    }
}
