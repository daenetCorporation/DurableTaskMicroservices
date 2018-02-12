using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using Daenet.DurableTaskMicroservices.Common.Entities;
using Daenet.Microservice.Common.Test.CounterOrchestration;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.UnitTests
{
    public class CounterOrchestration : OrchestrationBase<CounterOrchestrationInput, Null>
    {       
        protected override async Task<Null> RunOrchestration(OrchestrationContext context, CounterOrchestrationInput input, ILogger logger)
        {
            Debug.WriteLine($"Counter: {input.Counter}");

            if (!context.IsReplaying)
                logger.LogInformation("Orchestration started.");

            await ScheduleTask<Null>(typeof(Task1), new Task1Input() { Text = "Text passed from Orchestration." });

            await ScheduleTask<Null>(typeof(Task2), new Task2Input() { Number = 2 });
            //await context.ScheduleTask<Null>(typeof(Task2), new Task2Input() { Number = 2 });
            
            Task.Delay(100).Wait();

            input.Counter--;
            if (input.Counter > 0)
            {
                logger.LogInformation("Orchestration will ContinueAsNew.");
                context.ContinueAsNew(input);
            }

            return new Null();
        }
    }
}
