using Daenet.Common.Logging;
using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.Entities;
using Daenet.DurableTaskMicroservices.Common.Logging;
using Daenet.DurableTaskMicroservices.Common.Rules;
using DurableTask;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.BaseClasses
{
    /// <summary>
    /// Used as base class for all orchestrations. It extends DTF original TaskOrchestration
    /// for number of useful features like Logging and logging context propagation.
    /// It also provides dynamic routing features.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public abstract class OrchestrationBase<TInput, TOutput> : TaskOrchestration<TOutput, TInput>
        where TInput : OrchestrationInput
        where TOutput : class
    {

        #region Properties

        private OrchestrationContext m_OrchestrationConext;

        private TInput m_OrchestrationInput;

        private OrchestrationConfig m_Config;

        /// <summary>
        /// Gets the configuration of the task.
        /// </summary>
        public OrchestrationConfig Configuration
        {
            get
            {
                if (m_Config == null)
                {
                    var svc = ServiceHost.GetServiceConfiguration(String.Empty, this.GetType());
                    if (svc == null)
                        throw new InvalidOperationException(String.Format("Orchestration '{0}' has no configuration defined.", this.GetType().Name));

                    m_Config = svc.ServiceConfiguration as OrchestrationConfig;
                    if (m_Config == null)
                        m_Config = new OrchestrationConfig();
                }

                return m_Config;
            }
        }


        /// <summary>
        /// Returns the typed configuration of the task.
        /// </summary>
        /// <typeparam name="TConfig">Configuration type.</typeparam>
        /// <returns>Configuration as provided by host.</returns>
        public TConfig GetConfiguration<TConfig>() where TConfig : OrchestrationConfig
        {
            return this.Configuration as TConfig;
        }


       
        /// <summary>
        /// Instance of the daenet's LogManager component.
        /// </summary>
        protected ILogManager LogManager
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

        #endregion

        /*
        /// <summary>
        /// Sets the logging context as parent context for all configured tasks.
        /// This method enumerates all properties of type TaskBase
        /// </summary>
        /// <param name="orchestrationInput">The instance of Orchestration, which holds Task configuration properties.</param>
        /// <param name="logCtx">The context of orchestration, which should be set as </param>
        protected void SetLoggingContextOnTasks(OrchestrationInput orchestrationInput, LoggingContext logCtx)
        {
            if (logCtx != null)
            {
                var tp = orchestrationInput.GetType();
                var props = tp.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (var prop in props)
                {
                    if (prop.PropertyType.IsSubclassOf(typeof(TaskInput)))
                    {
                        dynamic val = (prop.GetValue(orchestrationInput));
                        if (val != null)
                            val.ParentLoggingContext = logCtx;
                    }
                }
            }
        }
        */
        
        /// <summary>
        /// Invoked when orchestration has to be started.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="orchestrationInput"></param>
        /// <returns></returns>
        public override async Task<TOutput> RunTask(OrchestrationContext context, TInput orchestrationInput)
        {
            TOutput result = null;

            string activityId = ServiceHost.GetActivityIdFromContext(orchestrationInput.Context);

            var logger = ServiceHost.GetLogger(this.GetType(), activityId);

            logger.BeginScope(context.OrchestrationInstance.InstanceId);

            logger.BeginScope(context.OrchestrationInstance.ExecutionId);

            try
            {
                if(context.IsReplaying == false)
                    logger?.LogDebug("OrchestrationBase {InstanceId} entered.", context.OrchestrationInstance.InstanceId);
                else
                    logger?.LogTrace("OrchestrationBase {InstanceId} entered.", context.OrchestrationInstance.InstanceId);

                m_OrchestrationConext = context;

                m_OrchestrationInput = orchestrationInput;

                result = await RunOrchestration(context, orchestrationInput, logger);

                if (context.IsReplaying == false)
                    logger?.LogDebug("OrchestrationBase {InstanceId} exited successfully", this.GetType().FullName);
                else
                    logger?.LogTrace("OrchestrationBase {InstanceId} exited successfully", this.GetType().FullName);

            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                throw;
            }
           
            return result;
        }

 

        /// <summary>
        ///  Create a SubOrchestration of the specified type. Also retry on failure as 
        ///  per supplied policy.
        ///  It transfers the input orchestration context to the new orchestration.
        ///  </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orchestrationType"></param>
        /// <param name="retryOptions"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<T> CreateSubOrchestrationInstanceWithRetry<T>(Type orchestrationType, RetryOptions retryOptions, OrchestrationInput input)
        {
            input.Context = m_OrchestrationInput.Context;
            return await m_OrchestrationConext.CreateSubOrchestrationInstanceWithRetry<T>(orchestrationType, 
                retryOptions, input);
        }


        /// <summary>
        /// Schedules specified task. It sets the ParentLoggingContext on context of orchestration.
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="taskInput"></param>
        /// <returns></returns>
        protected async Task<TTaskOutput> ScheduleTask<TTaskOutput>(Type taskType, TaskInput taskInput) where TTaskOutput : class
        {
            taskInput.Context = m_OrchestrationInput.Context;
            return await m_OrchestrationConext.ScheduleTask<TTaskOutput>(taskType, taskInput);
        }


        /// <summary>
        /// Abstract method which is called when concrete orchestration has to be started.
        /// </summary>
        /// <param name="context">OrchestrationContext</param>
        /// <param name="input">Orchestration input instance.</param>
        /// <param name="logger">Logger instance used for logging.</param>
        /// <returns></returns>
        protected abstract Task<TOutput> RunOrchestration(OrchestrationContext context, TInput input, ILogger logger);


        /// <summary>
        /// This method schedules dynamically set of rules as defined in orchestrationInput.RoutingRules.
        /// It can be optionally used to simplify execution of orchestration.
        /// RoutingRules implement usually some kind of Business Rule Engine.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        protected async Task<List<RoutingRuleResult>> ScheduleRoutedTasks(OrchestrationContext context,
            TaskInput instance)
        {
            List<RoutingRuleResult> results = new List<RoutingRuleResult>();

            RoutingManager mgr = new RoutingManager(context);

            var tasks = mgr.ScheduleRoutedTasks(instance, this.Configuration.RoutingRulesPipeline);

            await Task.WhenAll(tasks.ToArray());

            //bool hasFaulted = false;

            foreach (var task in tasks)
            {
                RoutingRuleResult res;

                if (task.IsFaulted)
                {
                    res = new RoutingRuleResult();
                    res.Exception = task.Exception.InnerExceptions.First().ToString();
                    //hasFaulted = true;
                }
                else
                {
                    res = task.Result;
                }

                results.Add(res);
            }

            //if (hasFaulted)
            //    throw new Exception("One or more routed tasks has failed.");

            return results;
        }
    }
}
