using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using DurableTask;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.UnitTests
{
    public class CounterOrchestration : OrchestrationBase<CounterOrchestrationInput, Null>
    {       
        protected override async Task<Null> RunOrchestration(OrchestrationContext context, CounterOrchestrationInput input)
        {
            int cnt = input.Counter;

            while (cnt > 0)
            {
                cnt--;

                await context.ScheduleTask<Null>(typeof(Task1), ":)");

                await context.ScheduleTask<Null>(typeof(Task2), ":<");

                Task.Delay(input.Delay).Wait();
            }

            return new Null();
        }
    }
}
