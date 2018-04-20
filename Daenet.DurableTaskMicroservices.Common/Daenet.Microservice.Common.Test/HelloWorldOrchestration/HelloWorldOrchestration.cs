using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using Daenet.Microservice.Common.Test.CounterOrchestration;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.Microservice.Common.Test.HelloWorldOrchestration
{
    internal class HelloWorldOrchestration : OrchestrationBase<HelloWorldOrchestrationInput, Null>
    {
        protected async override Task<Null> RunOrchestration(OrchestrationContext context, HelloWorldOrchestrationInput input, ILogger logger)
        {
            logger.LogInformation($"Orchestration Input: {input.HelloText}. IsReplaying: {context.IsReplaying}");

            await context.ScheduleTask<Null>(typeof(DurableTaskMicroservices.UnitTests.Task1), new Task1Input { Text = input.HelloText });

            return new Null();
        }
    }
}
