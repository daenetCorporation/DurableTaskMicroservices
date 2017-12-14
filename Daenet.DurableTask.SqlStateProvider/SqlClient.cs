using DurableTask.Core;
using DurableTask.Core.Tracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTask.SqlStateProvider
{
    public class SqlClient
    {
        private string m_ConnectionString;
        private readonly string m_BaseTable;
        private string m_SchemaName;

        public string StateTableWithSchema
        {
            get { return String.Format("{0}.{1}", m_SchemaName, m_BaseTable + "_State"); }
        }

        public string HistoryTableWithSchema
        {
            get { return String.Format("{0}.{1}", m_SchemaName, m_BaseTable + "_History"); }
        }

        public string StateTableName
        {
            get { return String.Format("{0}", m_BaseTable + "_State"); }
        }

        public string HistoryTableName
        {
            get { return String.Format("{0}", m_BaseTable + "_History"); }
        }

        public SqlClient(string baseTableName, string sqlConnectionString, string schemaName = "dbo")
        {
            if (string.IsNullOrEmpty(sqlConnectionString))
                throw new ArgumentException("ConnectionString is Null or Empty!", nameof(sqlConnectionString));

            if (string.IsNullOrEmpty(baseTableName))
                throw new ArgumentException("BaseTableName is Null or Empty!", nameof(baseTableName));

            m_ConnectionString = sqlConnectionString;
            m_BaseTable = baseTableName;

            m_SchemaName = schemaName;
        }

        /// <summary>
        /// Creates SQL Tables (History + State)
        /// </summary>
        /// <returns></returns>
        public async Task CreateStoreIfNotExistsAsync()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(m_ConnectionString))
                {
                    await con.OpenAsync();

                    SqlCommand cmd = con.CreateCommand();
                    cmd.Transaction = con.BeginTransaction();

                    StringBuilder command = new StringBuilder();

                    //// get default schema name
                    //cmd.CommandText = "SELECT SCHEMA_NAME()";

                    //var dbschema = await cmd.ExecuteScalarAsync();

                    //if (dbschema != null)
                    //    schema = dbschema.ToString();

                    //
                    // History Table
                    command.AppendFormat(@"IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES
                 WHERE TABLE_SCHEMA = '{0}'
                 AND  TABLE_NAME = '{1}'))", m_SchemaName, StateTableName);

                    command.AppendFormat(@"BEGIN 
                                         CREATE TABLE {0}
                                        (
	                                        [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
                                            [InstanceId] NVARCHAR(50) NOT NULL, 
                                            [ExecutionId] NVARCHAR(50) NOT NULL, 
                                            [CompletedTime] DATETIME NOT NULL, 
                                            [CompressedSize] BIGINT NOT NULL, 
                                            [CreatedTime] DATETIME NOT NULL, 
                                            [Input] NVARCHAR(MAX) NOT NULL, 
                                            [LastUpdatedTime] DATETIME NOT NULL, 
                                            [Name] NVARCHAR(100) NOT NULL, 
                                            [OrchestrationInstance] NVARCHAR(MAX) NOT NULL, 
                                            [OrchestrationStatus] NVARCHAR(MAX) NOT NULL, 
                                            [Output] NVARCHAR(MAX) NULL, 
                                            [ParentInstance] NVARCHAR(MAX) NOT NULL, 
                                            [Size] BIGINT NOT NULL, 
                                            [Status] NVARCHAR(50) NOT NULL, 
                                            [Tags] NVARCHAR(50) NULL, 
                                            [Version] NVARCHAR(50) NOT NULL
                                        )
                                          END", StateTableWithSchema);
                    command.AppendLine();

                    //
                    // State table
                    command.AppendFormat(@"IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES
                 WHERE TABLE_SCHEMA = '{0}'
                 AND  TABLE_NAME = '{1}'))", m_SchemaName, HistoryTableName);

                    command.AppendFormat(@"BEGIN 
                                        CREATE TABLE {0}
                                        (
	                                        [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
                                            [InstanceId] NVARCHAR(50) NOT NULL, 
                                            [ExecutionId] NVARCHAR(50) NOT NULL, 
                                            [SequenceNumber] INT NOT NULL, 
                                            [HistoryEvent] NVARCHAR(MAX) NOT NULL, 
                                            [TaskTimeStamp] DATETIME NOT NULL,
                                            [TimeStamp] DATETIMEOFFSET NOT NULL
                                        );
                                    END", HistoryTableWithSchema);


                    // set commandtext
                    cmd.CommandText = command.ToString();

                    await cmd.ExecuteNonQueryAsync();

                    cmd.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes SQL Tables from database
        /// </summary>
        /// <returns></returns>
        public async Task DeleteStoreIfExistsAsync()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(m_ConnectionString))
                {
                    string schema = "dbo";

                    await con.OpenAsync();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.Transaction = con.BeginTransaction();

                    // get default schema name
                    cmd.CommandText = "SELECT SCHEMA_NAME()";

                    var dbschema = await cmd.ExecuteScalarAsync();

                    if (dbschema != null)
                        schema = dbschema.ToString();

                    // build command for creating tables if not exists
                    StringBuilder command = new StringBuilder();
                    command.AppendFormat(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES
                 WHERE TABLE_SCHEMA = '{0}'
                 AND  TABLE_NAME = '{1}'))", schema, HistoryTableName);
                    command.AppendFormat(@"BEGIN Drop Table {0} END", HistoryTableWithSchema);
                    command.AppendLine();

                    command.AppendFormat(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES
                 WHERE TABLE_SCHEMA = '{0}'
                 AND  TABLE_NAME = '{1}'))", schema, StateTableName);
                    command.AppendFormat(@"BEGIN Drop Table {0} END", StateTableWithSchema);

                    cmd.CommandText = command.ToString();

                    await cmd.ExecuteNonQueryAsync();

                    cmd.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task WriteWorkItemAsync(IEnumerable<OrchestrationWorkItemInstanceEntity> entities)
        {
            if (entities == null || entities.Count() == 0)
                return;

            try
            {
                // TODO: use transaction here or not?
                using (SqlConnection con = new SqlConnection(m_ConnectionString))
                {
                    await con.OpenAsync();

                    SqlCommand cmd = con.CreateCommand();

                    cmd.CommandText = String.Format("Insert Into {0} (InstanceId, ExecutionId, SequenceNumber, HistoryEvent, EventTimestamp, TimeStamp) VALUES (@InstanceId, @ExecutionId, @SequenceNumber, @HistoryEvent, @EventTimestamp, @TimeStamp)", HistoryTableWithSchema);

                    foreach (var entity in entities)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(createSqlParam("@InstanceId", entity.InstanceId));
                        cmd.Parameters.Add(createSqlParam("@ExecutionId", entity.ExecutionId));
                        cmd.Parameters.Add(createSqlParam("@SequenceNumber", entity.SequenceNumber));
                        cmd.Parameters.Add(createSqlParam("@HistoryEvent", serializeToJson(entity.HistoryEvent)));
                        cmd.Parameters.Add(createSqlParam("@EventTimestamp", entity.EventTimestamp));
                        cmd.Parameters.Add(createSqlParam("@TimeStamp", DateTime.UtcNow));

                        if (await cmd.ExecuteNonQueryAsync() != 1)
                            throw new Exception("Insert of Entity failed to SQL State Provider!");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Write states into SQl State table
        /// </summary>
        /// <param name="stateInstances"></param>
        /// <returns></returns>
        public async Task WriteStateAsync(IEnumerable<OrchestrationStateInstanceEntity> stateInstances)
        {
            if (stateInstances == null || stateInstances.Count() == 0)
                return;

            using (SqlConnection con = new SqlConnection(m_ConnectionString))
            {
                await con.OpenAsync();

                SqlCommand cmd = con.CreateCommand();

                string insertCmdtxt = String.Format("Insert Into {0} (InstanceId, ExecutionId, CompletedTime, CompressedSize, CreatedTime, Input, LastUpdatedTime, Name, OrchestrationInstance, OrchestrationStatus, Output, ParentInstance, Size, Status, Tags, Version) VALUES (@InstanceId, @ExecutionId, @CompletedTime, @CompressedSize, @CreatedTime, @Input, @LastUpdatedTime, @Name, @OrchestrationInstance, @OrchestrationStatus, @Output, @ParentInstance, @Size, @Status, @Tags, @Version)", StateTableWithSchema);

                string updateCmdTxt = String.Format("UPDATE {0} SET CompletedTime = @CompletedTime, CompressedSize = @CompressedSize, CreatedTime = @CreatedTime, Input = @Input, LastUpdatedTime = @LastUpdatedTime, Name = @Name, OrchestrationInstance = @OrchestrationInstance, OrchestrationStatus = @OrchestrationStatus, Output = @Output, ParentInstance = @ParentInstance, Size = @Size, Status = @Status, Tags = @Tags, Version = @Version WHERE InstanceId = @InstanceId AND ExecutionID = @ExecutionId", StateTableWithSchema);

                string getCmdtxt = String.Format("Select * FROM {0} WHERE InstanceId = @InstanceId AND ExecutionId = @ExecutionId", StateTableWithSchema);


                foreach (var stateInstance in stateInstances)
                {
                    var state = stateInstance.State;

                    bool insert = true;
                    string instanceId = null;
                    string executionId = null;

                    if (state != null && state.OrchestrationInstance != null)
                    {
                        instanceId = state.OrchestrationInstance.InstanceId;
                        executionId = state.OrchestrationInstance.ExecutionId;
                    }

                    if (!String.IsNullOrEmpty(instanceId) && !String.IsNullOrEmpty(executionId))
                    {
                        // check if state already exists
                        cmd.CommandText = getCmdtxt;
                        cmd.Parameters.Add(createSqlParam("@InstanceId", instanceId));
                        cmd.Parameters.Add(createSqlParam("@ExecutionId", executionId));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                                insert = false;
                        }
                    }

                    if (insert)
                        cmd.CommandText = insertCmdtxt;
                    else
                        cmd.CommandText = updateCmdTxt;


                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(createSqlParam("@InstanceId", instanceId));
                    cmd.Parameters.Add(createSqlParam("@ExecutionId", executionId));
                    cmd.Parameters.Add(createSqlParam("@CompletedTime", state.CompletedTime));
                    cmd.Parameters.Add(createSqlParam("@CompressedSize", state.CompressedSize));
                    cmd.Parameters.Add(createSqlParam("@CreatedTime", state.CreatedTime));
                    cmd.Parameters.Add(createSqlParam("@Input", state.Input));
                    cmd.Parameters.Add(createSqlParam("@LastUpdatedTime", state.LastUpdatedTime));
                    cmd.Parameters.Add(createSqlParam("@Name", state.Name));
                    cmd.Parameters.Add(createSqlParam("@OrchestrationInstance", serializeToJson(state.OrchestrationInstance))); // serialize to json
                    cmd.Parameters.Add(createSqlParam("@OrchestrationStatus", state.OrchestrationStatus.ToString())); // serialize to json
                    cmd.Parameters.Add(createSqlParam("@Output", state.Output));
                    cmd.Parameters.Add(createSqlParam("@ParentInstance", serializeToJson(state.ParentInstance))); // serialize to json
                    cmd.Parameters.Add(createSqlParam("@Size", state.Size));
                    cmd.Parameters.Add(createSqlParam("@Status", state.Status));
                    cmd.Parameters.Add(createSqlParam("@Tags", serializeToJson(state.Tags)));
                    cmd.Parameters.Add(createSqlParam("@Version", state.Version));

                    if (await cmd.ExecuteNonQueryAsync() != 1)
                        throw new Exception("Insert of Entity failed to SQL State Provider!");
                }
            }
        }

        /// <summary>
        /// First looks up all orchestration states, which match specified time criteria.
        /// Then lookups all history events for found instances and deletes them from history events and from states.
        /// </summary>
        /// <param name="thresholdDateTimeUtc">Deletes instances older than.</param>
        /// <param name="timeRangeFilterType">Time lookup criteria.</param>
        /// <returns></returns>
        public async Task PurgeOrchestrationInstanceHistoryAsync(DateTime thresholdDateTimeUtc, OrchestrationStateTimeRangeFilterType timeRangeFilterType)
        {
            string columnName = null;

            //
            // get table column which matches the filter type
            switch (timeRangeFilterType)
            {
                case OrchestrationStateTimeRangeFilterType.OrchestrationCompletedTimeFilter:
                    columnName = "CompletedTime";
                    break;
                case OrchestrationStateTimeRangeFilterType.OrchestrationCreatedTimeFilter:
                    columnName = "CreatedTime";
                    break;
                case OrchestrationStateTimeRangeFilterType.OrchestrationLastUpdatedTimeFilter:
                    columnName = "LastUpdated";
                    break;
            }

            using (SqlConnection con = new SqlConnection(m_ConnectionString))
            {
                SqlTransaction trans = null;

                try
                {
                    await con.OpenAsync();
                    // use transaction here
                    using (trans = con.BeginTransaction())
                    {

                        SqlCommand cmd = con.CreateCommand();
                        cmd.Transaction = trans;

                        cmd.CommandText = String.Format("Select [OrchestrationInstance] from {0} WHERE {1} <= @DT", StateTableWithSchema, columnName);

                        cmd.Parameters.Add(createSqlParam("@DT", thresholdDateTimeUtc));

                        List<OrchestrationState> states = new List<OrchestrationState>();

                        // get all states
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                OrchestrationState state = new OrchestrationState();
                                //state.CompletedTime = DateTime.Parse(reader["CompletedTime"].ToString());
                                //state.CompressedSize = Int64.Parse(reader["CompressedSize"].ToString());

                                // only fill OrchestrationInstance for SequenceId and ExecutionId
                                state.OrchestrationInstance = JsonConvert.DeserializeObject<OrchestrationInstance>(reader["OrchestrationInstance"].ToString());

                                states.Add(state);
                            }
                        }

                        // delete all states which matches
                        cmd.CommandText = String.Format("DELETE from {0} WHERE {1} <= @DT", StateTableWithSchema, columnName);
                        await cmd.ExecuteNonQueryAsync();

                        // clear after execute
                        cmd.Parameters.Clear();

                        cmd.CommandText = String.Format("DELETE FROM {0} WHERE InstanceId = @InstanceId AND ExecutionId = @ExecutionId;", HistoryTableWithSchema);

                        // delete all history events where SequenceId and ExeuctionId appears in state list and delete those states afterwards
                        foreach (var state in states)
                        {
                            cmd.Parameters.Add(createSqlParam("@InstanceId", state.OrchestrationInstance.InstanceId));
                            cmd.Parameters.Add(createSqlParam("@ExecutionId", state.OrchestrationInstance.ExecutionId));

                            await cmd.ExecuteNonQueryAsync();

                            // clear after execute
                            cmd.Parameters.Clear();
                        }

                        trans.Commit();
                    }
                }
                catch (Exception)
                {
                    if (trans != null)
                        trans.Rollback();

                    throw;
                }
            }
        }

        public async Task<IEnumerable<OrchestrationHistoryEvent>> ReadOrchestrationHistoryEventsAsync(string instanceId, string executionId)
        {
            try
            {
                List<OrchestrationHistoryEvent> events = new List<OrchestrationHistoryEvent>();

                using (SqlConnection con = new SqlConnection(m_ConnectionString))
                {
                    await con.OpenAsync();

                    SqlCommand cmd = con.CreateCommand();

                    cmd.CommandText = String.Format("Select * from {0} WHERE InstanceId = @InstanceId AND ExecutionId = @ExecutionId;", StateTableWithSchema);

                    cmd.Parameters.Add(createSqlParam("@InstanceId", instanceId));
                    cmd.Parameters.Add(createSqlParam("@ExecutionId", executionId));

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            string instanced = reader["InstanceId"].ToString();
                            string executiond = reader["ExecutionId"].ToString();
                            int sequenceNumber = Int32.Parse(reader["SequenceNumber"].ToString());
                            DateTime tasktimeStamp = DateTime.Parse(reader["TaskTimeStamp"].ToString());
                            DateTimeOffset timeStamp = DateTimeOffset.Parse(reader["TimeStamp"].ToString());
                            string historyEventStr = reader["HistoryEvent"].ToString();

                            HistoryEvent hEvent = deserializeJson<HistoryEvent>(historyEventStr);

                            OrchestrationHistoryEvent ev = new OrchestrationHistoryEvent(instanced, executiond, sequenceNumber, tasktimeStamp, hEvent);
                            ev.Timestamp = timeStamp;

                            events.Add(ev);
                        }
                    }
                }

                return events;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<OrchestrationStateQuerySegment> QueryOrchestrationStatesAsync(OrchestrationStateQuery stateQuery, object continuationToken = null, int count = -1)
        {
            try
            {
                //throw new NotImplementedException("no access to OrchestrationStateQuery filters object");
                // no access to statequery filters objects
                OrchestrationStateQuerySegment segment = new OrchestrationStateQuerySegment();
                segment.Results = new List<OrchestrationState>();

                string query = "";

                var filters = stateQuery.GetFilters();


                OrchestrationStateQueryFilter primaryFilter = null;
                IEnumerable<OrchestrationStateQueryFilter> secondaryFilters = null;

                if (filters != null)
                {
                    primaryFilter = filters.Item1;
                    secondaryFilters = filters.Item2;
                }

                int skipRowsCount = 1;

                // if not set, set to 10.000 (10k)
                if (count == -1)
                    count = 10000;

                int toRow = count;

                // if token is set, get number from token where to start
                if (continuationToken != null && String.IsNullOrEmpty(continuationToken.ToString()) == false)
                {
                    skipRowsCount = deserializeJson<int>(continuationToken.ToString());
                    toRow = skipRowsCount + count - 1;
                }

                int paramCounter = 1;

                using (SqlConnection con = new SqlConnection(m_ConnectionString))
                {
                    SqlCommand cmd = con.CreateCommand();

                    await con.OpenAsync();


                    // basis string
                    //query.AppendFormat("SELECT TOP {0} * FROM {1}", count, StateTable);

                    //query.AppendFormat(@"SELECT *
                    //                    FROM
                    //                    (
                    //                    SELECT *, ROW_NUMBER() OVER (ORDER BY CreatedTime) rownum
                    //                    FROM {0} WHERE 1 = 1 {1}
                    //                    ) as seq
                    //                     WHERE seq.rownum BETWEEN {2} AND {3} ", StateTable, skipRowsCount, toRow);


                    //if (primaryFilter != null || secondaryFilters != null)
                    //    query.Append(" AND ");

                    StringBuilder whereQuery = new StringBuilder();

                    if (primaryFilter != null)
                    {
                        whereQuery.Append(" AND ");

                        whereQuery.Append(getFilterString(primaryFilter, cmd, paramCounter));
                        paramCounter++;
                    }

                    if (secondaryFilters != null)
                    {
                        foreach (var filter in secondaryFilters)
                        {
                            // add AND if something is in here
                            if (whereQuery.Length != 0)
                                whereQuery.Append(" AND ");

                            whereQuery.Append(getFilterString(filter, cmd, paramCounter));

                            paramCounter++;
                        }
                    }

                    //query.Append(whereQuery);

                    query = String.Format(@"SELECT *
                                        FROM
                                        (
                                        SELECT *, ROW_NUMBER() OVER (ORDER BY CreatedTime) rownum
                                        FROM {0} WHERE 1 = 1 {1} 
                                        ) as seq
                                         WHERE seq.rownum BETWEEN {2} AND {3} ", StateTableWithSchema, whereQuery, skipRowsCount, toRow);

                    //if (primaryFilter != null || secondaryFilters != null)
                    //    query.Append(" ORDER BY [CreatedTime] ASC ");

                    cmd.CommandText = query.ToString();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            OrchestrationState state = new OrchestrationState();
                            state.CompletedTime = DateTime.Parse(reader["CompletedTime"].ToString());
                            state.CompressedSize = Int64.Parse(reader["CompressedSize"].ToString());
                            state.CreatedTime = DateTime.Parse(reader["CreatedTime"].ToString());
                            state.Input = reader["Input"].ToString();
                            state.LastUpdatedTime = DateTime.Parse(reader["LastUpdatedTime"].ToString());
                            state.Name = reader["Name"].ToString();
                            state.OrchestrationInstance = deserializeJson<OrchestrationInstance>(reader["OrchestrationInstance"].ToString());
                            state.OrchestrationStatus = (OrchestrationStatus)Enum.Parse(typeof(OrchestrationStatus), reader["OrchestrationStatus"].ToString());
                            state.Output = reader["Output"].ToString();
                            state.ParentInstance = deserializeJson<ParentInstance>(reader["ParentInstance"].ToString());
                            state.Size = Int64.Parse(reader["Size"].ToString());
                            state.Status = reader["Status"].ToString();
                            state.Tags = deserializeJson<Dictionary<string, string>>(reader["Tags"].ToString());
                            state.Version = reader["Version"].ToString();

                            (segment.Results as List<OrchestrationState>).Add(state);
                        }
                    }

                    cmd.CommandText = String.Format("SELECT TOP 1 ROW_NUMBER() OVER (ORDER BY ID) rownum FROM {0}  WHERE 1 = 1 {1} ORDER BY rownum DESC", StateTableWithSchema, whereQuery.ToString());

                    var allRows = Convert.ToInt64(await cmd.ExecuteScalarAsync());

                    if (allRows != default(Int64))
                    {
                        // get actual queried items + skipped
                        long actualPosition = skipRowsCount + (segment.Results as List<OrchestrationState>).Count;

                        // if not all rows queried, return new position
                        if (actualPosition <= allRows)
                            segment.ContinuationToken = serializeToJson(actualPosition);
                        else
                            segment.ContinuationToken = null;
                    }
                }

                // skipRowsCount from parameter + added rows = new token
                //segment.ContinuationToken = serializeToJson((skipRowsCount + (segment.Results as List<OrchestrationState>).Count).ToString());

                return segment;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region Private Methods

        /// <summary>
        /// Deserializes a Json String to specified Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private T deserializeJson<T>(string jsonString)
        {
            object obj = JsonConvert.DeserializeObject(jsonString, typeof(T));

            return (T)obj;
        }

        /// <summary>
        /// Serialize History Event to json string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string serializeToJson(object obj)
        {
            string serializedHistoryEvent = JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Objects
                });

            return serializedHistoryEvent;
        }

        /// <summary>
        /// Create Sql Parameter object
        /// </summary>
        /// <param name="key">Name of Param in CommandText</param>
        /// <param name="val">Value of Param</param>
        /// <returns></returns>
        private SqlParameter createSqlParam(string key, object val)
        {
            SqlParameter param;

            if (val == null)
                param = new SqlParameter(key, DBNull.Value);
            else
                param = new SqlParameter(key, val);

            return param;
        }

        /// <summary>
        /// Builds the string for the query
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="cmd"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        private string getFilterString(OrchestrationStateQueryFilter filter, SqlCommand cmd, int number)
        {
            StringBuilder filterExpression = new StringBuilder();

            if (filter is OrchestrationStateInstanceFilter)
            {
                var typedFilter = filter as OrchestrationStateInstanceFilter;

                if (!String.IsNullOrEmpty(typedFilter.ExecutionId))
                {
                    filterExpression.AppendFormat(CultureInfo.InvariantCulture, SqlInstanceStoreConstants.ExecIdTemplate, number);
                    cmd.Parameters.Add(createSqlParam("@ExecutionId" + number, typedFilter.ExecutionId));
                }

                if (!String.IsNullOrEmpty(typedFilter.InstanceId))
                {
                    appendAnd(filterExpression);

                    if (typedFilter.StartsWith)
                    {
                        filterExpression.AppendFormat("InstanceId LIKE @InstanceId{0}", number);
                        cmd.Parameters.Add(createSqlParam("@InstanceId" + number, String.Format("{0}%", typedFilter.InstanceId)));
                    }
                    else
                    {
                        filterExpression.AppendFormat(CultureInfo.InvariantCulture, SqlInstanceStoreConstants.InstanceIdTemplate, number, number);

                        cmd.Parameters.Add(createSqlParam("@InstanceId" + number, typedFilter.InstanceId));
                    }
                }
            }
            else if (filter is OrchestrationStateStatusFilter)
            {
                var typedFilter = filter as OrchestrationStateStatusFilter;

                filterExpression.AppendFormat("OrchestrationStatus = @OrchestrationStatus{0}", number);

                cmd.Parameters.Add(createSqlParam("@OrchestrationStatus" + number, typedFilter.Status.ToString()));
            }
            else if (filter is OrchestrationStateNameVersionFilter)
            {
                var typedFilter = filter as OrchestrationStateNameVersionFilter;

                if (typedFilter.Name != null)
                {
                    filterExpression.AppendFormat("Name = @Name{0}", number);

                    cmd.Parameters.Add(createSqlParam("@Name" + number, typedFilter.Name));
                }

                if (typedFilter.Version != null)
                {
                    appendAnd(filterExpression);

                    filterExpression.AppendFormat("Version = @Version{0}", number);

                    cmd.Parameters.Add(createSqlParam("@Version" + number, typedFilter.Version));
                }
            }
            else if (filter is OrchestrationStateTimeRangeFilter)
            {
                var typedFilter = filter as OrchestrationStateTimeRangeFilter;

                switch (typedFilter.FilterType)
                {
                    case OrchestrationStateTimeRangeFilterType.OrchestrationCreatedTimeFilter:
                        {
                            filterExpression.AppendFormat("(CreatedTime >= @StartDatetime{0} AND CreatedTime < @EndDatetime{0})", number);
                            //cmd.Parameters.Add(createSqlParam("@OrchestrationStatus", typedFilter.o))
                            break;
                        }
                    case OrchestrationStateTimeRangeFilterType.OrchestrationCompletedTimeFilter:
                        {
                            filterExpression.AppendFormat("(CompletedTime >= @StartDatetime{0} AND CompletedTime < @EndDatetime{0} AND OrchestrationStatus = @OrchestrationStatus)", number);
                            cmd.Parameters.Add(createSqlParam("@OrchestrationStatus", OrchestrationStatus.Completed.ToString()));
                            break;
                        }
                    case OrchestrationStateTimeRangeFilterType.OrchestrationLastUpdatedTimeFilter:
                        {
                            filterExpression.AppendFormat("(LastUpdatedTime >= @StartDatetime{0} AND LastUpdatedTime < @EndDatetime{0})", number);
                            //cmd.Parameters.Add(createSqlParam("@OrchestrationStatus", OrchestrationStatus.Running))
                            break;
                        }
                    default:
                        throw new InvalidOperationException("Unsupported filter type: " + typedFilter.FilterType.GetType());
                }

                if (typedFilter.StartTime == DateTime.MinValue)
                    cmd.Parameters.Add(createSqlParam("@StartDatetime" + number, SqlDateTime.MinValue));
                else
                    cmd.Parameters.Add(createSqlParam("@StartDatetime" + number, typedFilter.StartTime));

                if (typedFilter.StartTime == DateTime.MinValue)
                    cmd.Parameters.Add(createSqlParam("@EndDatetime" + number, SqlDateTime.MaxValue));
                else
                    cmd.Parameters.Add(createSqlParam("@EndDatetime" + number, typedFilter.EndTime));

            }
            return filterExpression.ToString();
        }

        private static void appendAnd(StringBuilder filterExpression)
        {
            if (filterExpression.Length != 0)
                filterExpression.Append(" AND ");
        }

        #endregion
    }
}
