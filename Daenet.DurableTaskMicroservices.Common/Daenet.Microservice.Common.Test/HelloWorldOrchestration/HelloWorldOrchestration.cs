using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using Daenet.DurableTaskMicroservices.Common.Test.CounterOrchestration;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Daenet.DurableTaskMicroservices.Core;

namespace Daenet.DurableTaskMicroservices.Common.Test.HelloWorldOrchestration
{
    internal class HelloWorldOrchestration : OrchestrationBase<HelloWorldOrchestrationInput, Null>
    {
        protected async override Task<Null> RunOrchestration(OrchestrationContext context, HelloWorldOrchestrationInput input, ILogger logger)
        {
            logger.LogInformation($"Orchestration Input: {input.HelloText}. IsReplaying: {context.IsReplaying}");

            await this.ScheduleTask<Null>(typeof(DurableTaskMicroservices.UnitTests.Task1), new Task1Input { Text = input.HelloText });

            return new Null();
        }
    }
}
