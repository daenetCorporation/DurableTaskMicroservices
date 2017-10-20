using Daenet.DurableTask.Microservices;
using DurableTask;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.UnitTests
{
    public class CounterOrchestration : TaskOrchestration<int, CounterOrchetrationInput>
    {
        public async override Task<int> RunTask(OrchestrationContext context, CounterOrchetrationInput input)
        {
            int cnt = input.Counter;

            while (cnt > 0)
            {
                cnt--;

                await context.ScheduleTask<Null>(typeof(Task1), ":)");

                await context.ScheduleTask<Null>(typeof(Task2), ":<");

                Task.Delay(input.Delay).Wait();
            }

            return cnt;
        }
    }
}
