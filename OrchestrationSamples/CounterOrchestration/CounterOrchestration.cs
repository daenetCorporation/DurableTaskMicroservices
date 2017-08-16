using CounterOrchestration.Tasks;
using DurableTask;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

[assembly: Daenet.DurableTask.Microservices.IntegrationAssemblyAttribute]

namespace CounterOrchestration
{
    public class CounterOrchestration : TaskOrchestration<int, CounterOrchestrationInput>
    {
        public async override Task<int> RunTask(OrchestrationContext context, CounterOrchestrationInput input)
        {
            int cnt = input.Counter;

            while (cnt > 0)
            {
                cnt--;

                await context.ScheduleTask<string>(typeof(Task1), ":)");

                await context.ScheduleTask<string>(typeof(Task2), ":<");

                Task.Delay(input.Delay).Wait();
            }

            return cnt;
        }
    }
}
