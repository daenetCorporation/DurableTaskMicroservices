using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using DurableTask.Core;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.UnitTests
{
    public class CounterOrchestration : OrchestrationBase<CounterOrchestrationInput, Null>
    {       
        protected override async Task<Null> RunOrchestration(OrchestrationContext context, CounterOrchestrationInput input)
        {
            Debug.WriteLine($"{input.Counter}");

            await context.ScheduleTask<Null>(typeof(Task1), ":)");

            await context.ScheduleTask<Null>(typeof(Task2), ":<");

            Task.Delay(100).Wait();

            input.Counter--;
            if (input.Counter > 0)
            {
                context.ContinueAsNew(input);
            }

            return new Null();
        }
    }
}
