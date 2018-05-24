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
using Daenet.Common.Logging;
using Daenet.DurableTaskMicroservices.Common.Entities;
using Daenet.DurableTaskMicroservices.Common.Exceptions;
using Daenet.DurableTaskMicroservices.Core;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;

namespace Daenet.DurableTaskMicroservices.Common.Base
{
    /// <summary>
    /// Base class for tasks, which run as daenet.integration tasks.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public abstract class TaskBase<TInput, TOutput> : TaskActivity<TInput, TOutput>
        where TInput : TaskInput
        where TOutput : class
    {

        #region Properties

        /// <summary>
        /// Used to match instance specific configuration.
        /// </summary>
        public string InstanceName { get; set; }

        private TaskConfig m_Config;

        /// <summary>
        /// Gets the configuration of the task.
        /// </summary>
        //public TaskConfig Configuration
        //{
        //    get
        //    {
        //        if (m_Config == null)
        //        {
        //            m_Config = ServiceHost.GetActivityConfiguration(String.Empty, this.GetType()) as TaskConfig;
        //            //if (m_Config == null)
        //            //    throw new InvalidOperationException(String.Format("Task '{0}' has no defined configuration of type 'TaskConfig'. Found config entry with type '{1}'", this.GetType().Name, m_Config.GetType().Name));

        //            return m_Config;
        //        }

        //        return m_Config;
        //    }
        //}

        /// <summary>
        /// Get configuration for Task in specified orchestrationName
        /// </summary>
        /// <param name="orchestrationName"></param>
        /// <returns></returns>
        public TaskConfig GetConfiguration(object orchestrationName)
        {
            string name = orchestrationName.ToString();

            if (m_Config == null)
            {
                m_Config = ServiceHost.GetActivityConfiguration(name, this.GetType()) as TaskConfig;
                //if (m_Config == null)
                //    throw new InvalidOperationException(String.Format("Task '{0}' has no defined configuration of type 'TaskConfig'. Found config entry with type '{1}'", this.GetType().Name, m_Config.GetType().Name));

                // if still null, set to empty instance of TaskConfig
                if (m_Config == null)
                    m_Config = new TaskConfig();

                return m_Config;
            }

            return m_Config;
        }


        /// <summary>
        /// Returns the typed configuration of the task.
        /// </summary>
        /// <typeparam name="TConfig">Configuration type.</typeparam>
        /// <returns>Configuration as provided by host.</returns>
        public TConfig GetConfiguration<TConfig>(string orchestrationName) where TConfig : TaskConfig
        {
            return (TConfig)this.GetConfiguration(orchestrationName);
        }


        /// <summary>
        /// Instance of LogManager.
        /// </summary>
        public ILogManager LogManager
        {
            get
            {
                if (m_Log == null)
                {


                }

                return m_Log;
            }
            set
            {
                m_Log = value;
            }
        }

        #endregion

        #region Private Members

        private ILogManager m_Log;

        //private static List<string> m_TraceSourceInitialized = new List<string>();

        //private void initializeLog(TaskContext context, TaskInput inputArg)
        //{
        //    if (m_Log == null)
        //    {
        //        string logTraceSourceName = null;

        //        /*ILogManager parentLogMgr = new LogManager(inputArg.LoggerFactory, "not-used");

        //        LoggingContext parentLoggingContext = null;

        //        if (inputArg.Context.ContainsKey("ParentLoggingContext"))
        //        {
        //            parentLoggingContext = inputArg.Context["ParentLoggingContext"] as LoggingContext;

        //            if (parentLoggingContext?.LoggingScopes != null)
        //            {
        //                foreach (var scope in parentLoggingContext.LoggingScopes)
        //                {
        //                    // If Log Trace Source name is in parent context it will be used.
        //                    if (scope.Key == "LogTraceSourceName")
        //                        logTraceSourceName = scope.Value;

        //                    parentLogMgr.AddScope(scope.Key, scope.Value);
        //                }

        //                // Copy the previous SequenceId to ParentSequenceId to keep the track.
        //                if (parentLogMgr.CurrentScope.ContainsKey("SequenceId"))
        //                    parentLogMgr.AddScope("ParentSequenceId", parentLogMgr.CurrentScope["SequenceId"]);
        //                else
        //                    parentLogMgr.AddScope("ParentSequenceId", null);
        //            }
        //        }

        //        if (parentLoggingContext == null)
        //            parentLoggingContext = new LoggingContext();
                
        //        //
        //        // If log trace source name is specified in the configuration it will be used even if the context contains a parent logtrace source name.
        //        var cfg = this.GetConfiguration(inputArg.Orchestration);
        //        if (cfg != null)
        //            logTraceSourceName = cfg.LogTraceSourceName;

        //        if (String.IsNullOrEmpty(logTraceSourceName))
        //        {
        //            logTraceSourceName = this.GetType().FullName;
        //        }

        //        m_Log = new LogManager(inputArg.LoggerFactory, logTraceSourceName, parentLogMgr);

        //        // With new instance of the LogManager we always create a new SequenceId.
        //        m_Log.AddScope("SequenceId", Guid.NewGuid().ToString());

        //        // Add an new ActivityId if not existing yet. 
        //        if (!m_Log.CurrentScope.ContainsKey("ActivityId"))
        //            m_Log.AddScope("ActivityId", Guid.NewGuid().ToString());

        //        // Add OrchestrationInstanceId
        //        if (!m_Log.CurrentScope.ContainsKey("OrchestrationInstanceId"))
        //            m_Log.AddScope("OrchestrationInstanceId", context.OrchestrationInstance.InstanceId);
        //            */
        //    }
        //}

        #endregion
        protected override TOutput Execute(TaskContext context, TInput taskInputArgs)
        {
            TOutput result = null;

            string activityId = ServiceHost.GetActivityIdFromContext(taskInputArgs.Context);

            var logger = ServiceHost.GetLogger(this.GetType(), activityId);

            logger.BeginScope(context.OrchestrationInstance.InstanceId);

            logger.BeginScope(context.OrchestrationInstance.ExecutionId);

            try
            {
                logger?.LogDebug(EventIds.TaskBase.TaskStarted, "TaskBase: '{Task}' started successfully", this.GetType().FullName);
                
                result = RunTask(context, taskInputArgs, logger);

                logger?.LogDebug(EventIds.TaskBase.TaskEnded, "TaskBase: '{Task}' exited successfully", this.GetType().FullName);
            }
            catch (ValidationRuleException validationException)
            {
                var currentType = this.GetType();
                string taskType = currentType.Namespace + "." + currentType.Name;
                var validatedData = Newtonsoft.Json.JsonConvert.SerializeObject(validationException.ValidatingInstance);
                string validationRulesResult = String.Empty;
                foreach (var res in validationException.ValidationResult.Results)
                    validationRulesResult += Environment.NewLine + "Rule " + res.Key + "=" + res.Value.ToString();

                LogManager.TraceErrValidationRule(validationException, taskType, validatedData, validationRulesResult);

                throw;
            }
            catch (Exception ex)
            {
                logger?.LogError(EventIds.TaskBase.TaskFailed, ex, "Task failed.");
                throw;                
            }
            finally
            {

            }

            return result;
        }


        protected abstract TOutput RunTask(TaskContext context, TInput input, ILogger logger);
    }
}
