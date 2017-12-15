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

        public Task<object> DeleteEntitiesAsync(IEnumerable<InstanceEntityBase> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrchestrationStateInstanceEntity>> GetOrchestrationStateAsync(string instanceId, bool allInstances)
        {
            throw new NotImplementedException();
        }

        public Task<OrchestrationStateInstanceEntity> GetOrchestrationStateAsync(string instanceId, string executionId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrchestrationWorkItemInstanceEntity>> GetOrchestrationHistoryEventsAsync(string instanceId, string executionId)
        {
            throw new NotImplementedException();
        }

        public Task<int> PurgeOrchestrationHistoryEventsAsync(DateTime thresholdDateTimeUtc, OrchestrationStateTimeRangeFilterType timeRangeFilterType)
        {
            throw new NotImplementedException();
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

    }
}
