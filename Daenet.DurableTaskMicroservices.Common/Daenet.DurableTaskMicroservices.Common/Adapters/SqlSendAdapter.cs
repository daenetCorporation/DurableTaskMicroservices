using Daenet.DurableTaskMicroservices.Common;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using Daenet.DurableTaskMicroservices.Common.Entities;
using Daenet.DurableTaskMicroservices.Common.Logging;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;

namespace Daenet.DurableTaskMicroservices.Common.Adapters
{
    public class SqlSendAdapter<TAdapterOutput, TAdapterInput> : SendAdapterBase<TAdapterInput, TAdapterOutput> 
        where TAdapterOutput : class
        where TAdapterInput : TaskInput
    {


        protected override TAdapterOutput SendData(TaskContext context, TAdapterInput input, ILogger logger)
        {
            SqlSendAdapterConfig config = this.GetConfiguration<SqlSendAdapterConfig>(input.Context["Orchestration"].ToString());

            TAdapterOutput res = null;

            base.LogManager.TraceInfMethodStarted("Daenet.Integration.SqlSendAdapter.RunTask()");

            if (String.IsNullOrEmpty(config.ConnectionString))
                throw new Exception("SqlSendAdapter must have valid ConnectionString. Please check your configuration.");
            if (String.IsNullOrEmpty(config.MapperQualifiedName))
                throw new Exception("SqlSendAdapter must have valid MapperQualifiedName. Please check your configuration.");

            using (SqlConnection connection = new SqlConnection(config.ConnectionString))
            {
                var mapper = Factory.GetAdapterMapper(config.MapperQualifiedName);
                //SqlCommand sqlCmd = connection.CreateCommand();
                SqlCommand sqlCmd = (SqlCommand)mapper.Map(input.Data);

                connection.Open();

                // Start a local transaction.
                SqlTransaction transaction = connection.BeginTransaction("SqlSendAdapterTransaction");

                // Must assign both transaction object and connection to Command object for a pending local transaction
                sqlCmd.Connection = connection;
                sqlCmd.Transaction = transaction;
                
                try
                {
                    var nrOfRecords = sqlCmd.ExecuteNonQuery();

                    // Attempt to commit the transaction.
                    transaction.Commit();
                }
                catch (Exception ex)
                {   
                    // Attempt to roll back the transaction. 
                    try
                    {
                        base.LogManager.TraceErryAdapterExecution(ex, "Daenet.Integration.SqlSendAdapter");
                        transaction.Rollback();
                        base.LogManager.TraceInfTransactionRolledBack();
                        throw;
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred 
                        // on the server that would cause the rollback to fail, such as 
                        // a closed connection.
                        base.LogManager.TraceErrFailedToCommitTransactionRollback(ex2, "Daenet.Integration.SqlSendAdapter");
                        throw;

                    }
                }
            }

            base.LogManager.TraceInfMethodCompleted("Daenet.Integration.SqlSendAdapter.RunTask()");

            return res;
    
        }
    }
}
