using Daenet.Common.Logging;
using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.Entities;
using Daenet.DurableTaskMicroservices.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Extensions
{
    public static class HostExtensions
    {
        public static async Task StartServiceAsync(this ServiceHost host, string orchestrationQualifiedName,
            OrchestrationInput inputArgs, string activityId)
        {
            var tp = Type.GetType(orchestrationQualifiedName);
            if (tp == null)
                throw new ArgumentException(String.Format("Cannot lookup type '{0} ", orchestrationQualifiedName));

            Microservice config = ServiceHost.GetServiceConfiguration(String.Empty, tp);

            var svcCfg = config.ServiceConfiguration as OrchestrationConfig;
            if (svcCfg == null)
                throw new ArgumentException(String.Format("Specified orchestration configuration is not of type 'OrchestrationConfig'. Failed by starting of '{1}' - Specified (invalid) configuration type: '{2}'.", orchestrationQualifiedName, config));

            if (!String.IsNullOrEmpty(svcCfg.LogTraceSourceName))
            {
                foreach (var item in config.ActivityConfiguration)
                {
                    TaskConfig cfg = item.Value as TaskConfig;

                    //
                    // We can have configuration, which is not of type of TaskConfig.
                    if (cfg != null)
                    {
                        if (String.IsNullOrEmpty(cfg.LogTraceSourceName))
                        {
                            cfg.LogTraceSourceName = svcCfg.LogTraceSourceName;
                        }
                    }
                }
            }

            //
            // Generate new Guid if not set
            if (String.IsNullOrEmpty(activityId))
                activityId = Guid.NewGuid().ToString();

            inputArgs.Context = new Dictionary<string, object>();
            var logCtx = new LoggingContext();
            logCtx.LoggingScopes = new Dictionary<string, string>();
            logCtx.LoggingScopes.Add("ActivityId", activityId);
            inputArgs.Context.Add("Orchestration", config.Type.FullName);
            inputArgs.Context.Add("ParentLoggingContext", logCtx);


            await host.StartServiceAsync(orchestrationQualifiedName, inputArgs);
        }

        public static async Task RestartService(this ServiceHost host, string orchestrationQualifiedName,
           OrchestrationInput inputArgs, string activityId)
        {
            await host.StopServiceAsync(orchestrationQualifiedName);

            await host.StartServiceAsync(orchestrationQualifiedName, inputArgs, activityId);
        }
    }
}
