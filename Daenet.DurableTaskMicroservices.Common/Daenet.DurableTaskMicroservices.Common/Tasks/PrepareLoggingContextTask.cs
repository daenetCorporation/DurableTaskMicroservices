using Daenet.Common.Logging;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using Daenet.DurableTaskMicroservices.Common.Entities;
using DurableTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Tasks
{
    public class PrepareLoggingContextTask : TaskBase<PrepareLoggingContextTaskInput, LoggingContext>
    {

        private static List<string> m_TraceSourceInitialized = new List<string>();

        protected override LoggingContext RunTask(TaskContext context, PrepareLoggingContextTaskInput input)
        {
            if (String.IsNullOrEmpty(input.LogTraceSourceName) == false)
                this.GetConfiguration(input.Orchestration).LogTraceSourceName = input.LogTraceSourceName;

            if (String.IsNullOrEmpty(this.GetConfiguration(input.Context["Orchestration"]).LogTraceSourceName))
                this.GetConfiguration(input.Orchestration).LogTraceSourceName =  this.GetType().Namespace + "." + this.GetType().Name;
            
            LoggingContext parentLoggingContext = input.ParentLoggingContext;
            
            string traceSourceName = this.GetConfiguration(input.Orchestration).LogTraceSourceName;
            LogManager parentScope = new LogManager(input.LoggerFactory, traceSourceName);

            parentScope.AddScope("ParentSequenceId", null);

            if (parentLoggingContext != null && parentLoggingContext.LoggingScopes != null && parentLoggingContext.LoggingScopes.Count > 0)
            {
                foreach (var scope in parentLoggingContext.LoggingScopes)
                    parentScope.AddScope(scope.Key, scope.Value);

                // Copy the previous SequenceId to ParentSequenceId to keep the track.
                if (parentScope.CurrentScope.ContainsKey("SequenceId"))
                    parentScope.AddScope("ParentSequenceId", parentScope.CurrentScope["SequenceId"]);
            }

            LogManager logManager = new LogManager(input.LoggerFactory, traceSourceName);

            // With new instance of the LogManager we always create a new SequenceId.
            logManager.AddScope("SequenceId", Guid.NewGuid().ToString());

            // Add an new ActivityId if not existing yet. 
            if (!logManager.CurrentScope.ContainsKey("ActivityId"))
                logManager.AddScope("ActivityId", Guid.NewGuid().ToString());

            // Add WorkflowInstanceId as a new ActivityId if not existing yet. 
            if (!logManager.CurrentScope.ContainsKey("OrchestrationInstanceId"))
                logManager.AddScope("OrchestrationInstanceId", context.OrchestrationInstance.InstanceId);

            logManager.AddScope("LogTraceSourceName", this.GetConfiguration(input.Orchestration).LogTraceSourceName);

            lock (m_TraceSourceInitialized)
            {
                // All DALs will append its class name which will identify them as component.
                //m_Log.AddScope(ApiBase.CreateComponentScopeName() + ".dal", this.GetType().Name);

                if (!m_TraceSourceInitialized.Contains(traceSourceName))
                    m_TraceSourceInitialized.Add(traceSourceName);

              //  logManager.TraceMessage(TracingLevel.Level4, 0, "Logging Context prepared for the workflow instance with the Id=" + context.WorkflowInstanceId.ToString());
            }

            var newLoggingContext = new LoggingContext { LoggingScopes = logManager.CurrentScope };

           // copyScopes(input.ParentOrchestrationInput, newLoggingContext);

            return newLoggingContext;
        }

        /*
        /// <summary>
        /// Copies all scopes from input of parent orchestration to the new contexr.
        /// </summary>
        /// <param name="input"></param>
        private void copyScopes(OrchestrationInput input, LoggingContext parentLoggingContext)
        {
            if (input.IsInitialized)
                return;

            PropertyInfo[] orchestrationInputProps = input.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in orchestrationInputProps)
            {
                if (p.PropertyType.BaseType == typeof(TaskInput))
                {
                    // If ParentLoggingContext is null, then set it. 
                    if (((TaskInput)p.GetValue(input, null)) != null && ((TaskInput)p.GetValue(input, null)).ParentLoggingContext == null)
                        ((TaskInput)p.GetValue(input, null)).ParentLoggingContext = new LoggingContext { LoggingScopes = parentLoggingContext.LoggingScopes };

                    // If LogTraceSourceName is empty then set it.
                    if (this.Configuration != null && String.IsNullOrEmpty(this.Configuration.LogTraceSourceName))
                        this.Configuration.LogTraceSourceName = this.Configuration.LogTraceSourceName;

                }
            }
        }*/
    }
}

