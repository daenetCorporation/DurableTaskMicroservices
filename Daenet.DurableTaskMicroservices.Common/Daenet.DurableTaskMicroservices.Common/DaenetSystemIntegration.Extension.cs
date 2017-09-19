using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daenet.Common.Logging;

namespace Daenet.DurableTaskMicroservices.Common.Logging
{
    public static class Extension
    {
        /// <summary>
        /// Logs the message
        /// The message string: 'Starded execution of the methd {0} ...'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        /// <param name="FQNNameOfTheMethod">Full qualified name of the method.</param>
        public static void TraceInfMethodStarted(this ILogManager log, object FQNNameOfTheMethod )
        {
 
            log.TraceMessage(TracingLevel.Level3, DiagnosticEvents.InfMethodStarted, DaenetSystemIntegration.InfMethodStarted, new object[] {  FQNNameOfTheMethod,  });
         }

		        /// <summary>
        /// Logs the message
        /// The message string: 'Completed execution of the method {0}.'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        /// <param name="FQNNameOfTheMethod">Full qualified name of the method.</param>
        public static void TraceInfMethodCompleted(this ILogManager log, object FQNNameOfTheMethod )
        {
 
            log.TraceMessage(TracingLevel.Level3, DiagnosticEvents.InfMethodCompleted, DaenetSystemIntegration.InfMethodCompleted, new object[] {  FQNNameOfTheMethod,  });
         }

		        /// <summary>
        /// Logs the message
        /// The message string: 'The SQL Statement {0} defined by CheckDataCmdText has been successfully executed and delivered the value {1} as result.'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        /// <param name="CheckDataCmdText">SQL Statement defined by CheckDataCmdText.</param>
        /// <param name="CheckDataCmdTextResult">Result delivered by the SQL statement CheckDataCmdText.</param>
        public static void TraceInfCheckDataCmdTextResults(this ILogManager log, object CheckDataCmdText, object CheckDataCmdTextResult )
        {
 
            log.TraceMessage(TracingLevel.Level4, DiagnosticEvents.InfCheckDataCmdTextResults, DaenetSystemIntegration.InfCheckDataCmdTextResults, new object[] {  CheckDataCmdText,  CheckDataCmdTextResult,  });
         }

		        /// <summary>
        /// Logs the message
        /// The message string: 'The SQL Statement {0} defined by FetchDataCmdText has been successfully executed and delivered {1} as result.'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        /// <param name="FetchDataCmdText">SQL Statement defined by FetchDataCmdText.</param>
        /// <param name="FetchDataCmdTextResult">Result delivered by the SQL statement FetchDataCmdText.</param>
        public static void TraceInfFetchDataCmdTextResults(this ILogManager log, object FetchDataCmdText, object FetchDataCmdTextResult )
        {
 
            log.TraceMessage(TracingLevel.Level4, DiagnosticEvents.InfFetchDataCmdTextResults, DaenetSystemIntegration.InfFetchDataCmdTextResults, new object[] {  FetchDataCmdText,  FetchDataCmdTextResult,  });
         }

		        /// <summary>
        /// Logs the message
        /// The message string: 'Transaction rollback successfully commited.'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        public static void TraceInfTransactionRolledBack(this ILogManager log )
        {
 
            log.TraceMessage(TracingLevel.Level4, DiagnosticEvents.InfTransactionRolledBack, DaenetSystemIntegration.InfTransactionRolledBack, new object[] {  });
         }

		        /// <summary>
        /// Logs the message
        /// The message string: 'Failed to commit transaction rollback after some error in the method RunTask() of the adapter \"{0}\".'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        /// <param name="FQAdapterName">Full quallified adapter name.</param>
        public static void TraceErrFailedToCommitTransactionRollback(this ILogManager log, object FQAdapterName )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErrFailedToCommitTransactionRollback, DaenetSystemIntegration.ErrFailedToCommitTransactionRollback, new object[] {  FQAdapterName,  });
         }

		        /// <summary>
        /// 
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
		/// <param name="ex">The exception to log.</param>
        /// <param name="FQAdapterName"></param>
        public static void TraceErrFailedToCommitTransactionRollback(this ILogManager log, Exception ex, object FQAdapterName )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErrFailedToCommitTransactionRollback, ex, DaenetSystemIntegration.ErrFailedToCommitTransactionRollback, new object[] {  FQAdapterName,  });
         
        }        /// <summary>
        /// Logs the message
        /// The message string: 'An error occured while executing the RunTask() method of the adapter \"{0}\". Trying to rollback transaction ...'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        /// <param name="param0">FQAdapterName</param>
        public static void TraceErryAdapterExecution(this ILogManager log, object param0 )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErryAdapterExecution, DaenetSystemIntegration.ErryAdapterExecution, new object[] {  param0,  });
         }

		        /// <summary>
        /// 
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
		/// <param name="ex">The exception to log.</param>
        /// <param name="param0"></param>
        public static void TraceErryAdapterExecution(this ILogManager log, Exception ex, object param0 )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErryAdapterExecution, ex, DaenetSystemIntegration.ErryAdapterExecution, new object[] {  param0,  });
         
        }        /// <summary>
        /// Logs the message
        /// The message string: 'An validation exception occurred while executing the Task based on the type \"{0}\".\r\nThe data being validated are represented by the following string: \n\r\"{1}\".\r\nThe validation results are: {2}'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        /// <param name="TaskType">The parameter 0 for the String format</param>
        /// <param name="ValidationData">The parameter 1 for the String format</param>
        /// <param name="ValidationResults">The parameter 2 for the String format</param>
        public static void TraceErrValidationRule(this ILogManager log, object TaskType, object ValidationData, object ValidationResults )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErrValidationRule, DaenetSystemIntegration.ErrValidationRule, new object[] {  TaskType,  ValidationData,  ValidationResults,  });
         }

		        /// <summary>
        /// 
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
		/// <param name="ex">The exception to log.</param>
        /// <param name="TaskType"></param>
        /// <param name="ValidationData"></param>
        /// <param name="ValidationResults"></param>
        public static void TraceErrValidationRule(this ILogManager log, Exception ex, object TaskType, object ValidationData, object ValidationResults )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErrValidationRule, ex, DaenetSystemIntegration.ErrValidationRule, new object[] {  TaskType,  ValidationData,  ValidationResults,  });
         
        }        /// <summary>
        /// Logs the message
        /// The message string: 'An TaskFailed exception occurred during the execution of the orchestration {0}.'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        /// <param name="orchestrationName">Name of the failed orchestration.</param>
        public static void TraceErrTaskFailedException(this ILogManager log, object orchestrationName )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErrTaskFailedException, DaenetSystemIntegration.ErrTaskFailedException, new object[] {  orchestrationName,  });
         }

		        /// <summary>
        /// 
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
		/// <param name="ex">The exception to log.</param>
        /// <param name="orchestrationName"></param>
        public static void TraceErrTaskFailedException(this ILogManager log, Exception ex, object orchestrationName )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErrTaskFailedException, ex, DaenetSystemIntegration.ErrTaskFailedException, new object[] {  orchestrationName,  });
         
        }        /// <summary>
        /// Logs the message
        /// The message string: 'Compensation of the orchestration {0} has been failed.'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        /// <param name="orchestrationName">Name of the orchestration.</param>
        public static void TraceErrOrchestrationCompensationFailed(this ILogManager log, object orchestrationName )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErrOrchestrationCompensationFailed, DaenetSystemIntegration.ErrOrchestrationCompensationFailed, new object[] {  orchestrationName,  });
         }

		        /// <summary>
        /// 
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
		/// <param name="ex">The exception to log.</param>
        /// <param name="orchestrationName"></param>
        public static void TraceErrOrchestrationCompensationFailed(this ILogManager log, Exception ex, object orchestrationName )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErrOrchestrationCompensationFailed, ex, DaenetSystemIntegration.ErrOrchestrationCompensationFailed, new object[] {  orchestrationName,  });
         
        }        /// <summary>
        /// Logs the message
        /// The message string: 'Orchestration failed before LoggingContext has been initialized. InstanceId: '{0}' - '{1}'!'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        /// <param name="param0">The parameter 0 for the String format</param>
        /// <param name="param1">The parameter 1 for the String format</param>
        public static void TraceErrLoggingSystemFailed(this ILogManager log, object param0, object param1 )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErrLoggingSystemFailed, DaenetSystemIntegration.ErrLoggingSystemFailed, new object[] {  param0,  param1,  });
         }

		        /// <summary>
        /// 
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
		/// <param name="ex">The exception to log.</param>
        /// <param name="param0"></param>
        /// <param name="param1"></param>
        public static void TraceErrLoggingSystemFailed(this ILogManager log, Exception ex, object param0, object param1 )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErrLoggingSystemFailed, ex, DaenetSystemIntegration.ErrLoggingSystemFailed, new object[] {  param0,  param1,  });
         
        }        /// <summary>
        /// Logs the message
        /// The message string: 'Orchestration failed. InstanceId: '{0}' - '{1}'!'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        /// <param name="param0">The parameter 0 for the String format</param>
        /// <param name="param1">The parameter 1 for the String format</param>
        public static void TraceErrOrchestrationFailed(this ILogManager log, object param0, object param1 )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErrOrchestrationFailed, DaenetSystemIntegration.ErrOrchestrationFailed, new object[] {  param0,  param1,  });
         }

		        /// <summary>
        /// 
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
		/// <param name="ex">The exception to log.</param>
        /// <param name="param0"></param>
        /// <param name="param1"></param>
        public static void TraceErrOrchestrationFailed(this ILogManager log, Exception ex, object param0, object param1 )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErrOrchestrationFailed, ex, DaenetSystemIntegration.ErrOrchestrationFailed, new object[] {  param0,  param1,  });
         
        }        /// <summary>
        /// Logs the message
        /// The message string: 'FileReceiveAdapter Error'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        public static void TraceErrFileReceiveAdapter(this ILogManager log )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErrFileReceiveAdapter, DaenetSystemIntegration.ErrFileReceiveAdapter, new object[] {  });
         }

		        /// <summary>
        /// 
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
		/// <param name="ex">The exception to log.</param>
        public static void TraceErrFileReceiveAdapter(this ILogManager log, Exception ex )
        {
 
            log.TraceError(TracingLevel.Level1, DiagnosticEvents.ErrFileReceiveAdapter, ex, DaenetSystemIntegration.ErrFileReceiveAdapter, new object[] {  });
         
        }        /// <summary>
        /// Logs the message
        /// The message string: 'FileReceiveAdapter Start. Input FileMask: {0}, Folder: {1}'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        /// <param name="param0">The parameter 0 for the String format</param>
        /// <param name="param1">The parameter 1 for the String format</param>
        public static void TraceInfStartFileReceiveAdapter(this ILogManager log, object param0, object param1 )
        {
 
            log.TraceMessage(TracingLevel.Level4, DiagnosticEvents.InfStartFileReceiveAdapter, DaenetSystemIntegration.InfStartFileReceiveAdapter, new object[] {  param0,  param1,  });
         }

		        /// <summary>
        /// Logs the message
        /// The message string: 'FileReceiveAdapter End. File: {0}'
        /// </summary>
        /// <param name="log">The Log Manager, which will be extended.</param>
        /// <param name="param0">The parameter 0 for the String format</param>
        public static void TraceInfEndFileReceiveAdapter(this ILogManager log, object param0 )
        {
 
            log.TraceMessage(TracingLevel.Level4, DiagnosticEvents.InfEndFileReceiveAdapter, DaenetSystemIntegration.InfEndFileReceiveAdapter, new object[] {  param0,  });
         }

		}
}
