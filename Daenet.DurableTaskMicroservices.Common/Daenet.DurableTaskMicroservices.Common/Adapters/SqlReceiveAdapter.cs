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
using Daenet.DurableTaskMicroservices.Common;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using Daenet.DurableTaskMicroservices.Common.Entities;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;

namespace Daenet.DurableTaskMicroservices.Common.Adapters
{
    public class SqlReceiveAdapter<TAdapterOutput, TAdapterInput> : ReceiveAdapterBase<TAdapterInput, TAdapterOutput> 
        where TAdapterOutput : class
        where TAdapterInput : TaskInput
    {
        protected override TAdapterOutput ReceiveData(TaskContext context, TAdapterInput taskInput, ILogger logger)
        {
            SqlReceiveAdapterConfig config = this.GetConfiguration<SqlReceiveAdapterConfig>(taskInput.Orchestration);

            TAdapterOutput res = null;

            base.LogManager.TraceInfMethodStarted("Daenet.Integration.SqlReceiveAdapter.RunTask()");
            
            if (String.IsNullOrEmpty(config.ConnectionString))
                throw new Exception("SqlReceiveAdapter must have valid ConnectionString. Please check your configuration.");
            if (String.IsNullOrEmpty(config.CheckDataCmdText))
                throw new Exception("SqlReceiveAdapter must have valid CheckDataCmdText. Please check your configuration.");
            if (String.IsNullOrEmpty(config.FetchDataCmdText))
                throw new Exception("SqlReceiveAdapter must have valid FetchDataCmdText. Please check your configuration.");
            if (String.IsNullOrEmpty(config.MapperQualifiedName))
                throw new Exception("SqlReceiveAdapter must have valid MapperQualifiedName. Please check your configuration.");

            using (SqlConnection connection = new SqlConnection(config.ConnectionString))
            {
                connection.Open();

                SqlCommand checkCmd = connection.CreateCommand();


                // Start a local transaction.
                SqlTransaction transaction = connection.BeginTransaction("SqlReceiveAdapterTransaction");

                // Must assign both transaction object and connection to Command object for a pending local transaction
                checkCmd.Connection = connection;
                checkCmd.Transaction = transaction;
                checkCmd.CommandText = config.CheckDataCmdText;

                try
                {
                    var nrOfRecords = checkCmd.ExecuteScalar();

                    if (!(nrOfRecords is int))
                    {
                        throw new Exception("The SqlReceiveAdapter SQL command defined with CheckDataCmdText must return numeric value greater or equal zero. Only if the returned value is greater zero, then the command defined by FetchDataCmdText is going to be executed.");
                    }
                    else
                    {
                        base.LogManager.TraceInfCheckDataCmdTextResults(checkCmd.CommandText, nrOfRecords.ToString());

                        // Only if the CheckDataCmdText statement delivered value greater 0, then we should execute the FetchDataCmdText command to get data.
                        if ((nrOfRecords as int?).HasValue && (nrOfRecords as int?).Value > 0)
                        {

                            SqlCommand fetchCmd = connection.CreateCommand();
                            fetchCmd.Connection = connection;
                            fetchCmd.Transaction = transaction;
                            fetchCmd.CommandText = config.FetchDataCmdText;

                            SqlDataReader reader = fetchCmd.ExecuteReader();

                            base.LogManager.TraceInfFetchDataCmdTextResults(fetchCmd.CommandText, reader.RecordsAffected.ToString());

                            var mapper = Factory.GetAdapterMapper(config.MapperQualifiedName);

                            res = (TAdapterOutput)mapper.Map(reader);
                        }

                    }

                    // Attempt to commit the transaction.
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Attempt to roll back the transaction. 
                    try
                    {
                        base.LogManager.TraceErryAdapterExecution(ex, "Daenet.Integration.ReceiveSqlAdapter");
                        transaction.Rollback();
                        base.LogManager.TraceInfTransactionRolledBack();
                        throw;
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred 
                        // on the server that would cause the rollback to fail, such as 
                        // a closed connection.
                        base.LogManager.TraceErrFailedToCommitTransactionRollback(ex2, "Daenet.Integration.SqlReceiveAdapter");
                        throw;
                        
                    }
                }
            }

            base.LogManager.TraceInfMethodCompleted("Daenet.Integration.SqlReceiveAdapter.RunTask()");

            return res;
        }
    }
}
