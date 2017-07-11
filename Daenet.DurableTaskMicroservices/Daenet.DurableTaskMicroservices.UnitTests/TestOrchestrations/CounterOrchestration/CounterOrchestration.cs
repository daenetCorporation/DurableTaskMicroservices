using DurableTask;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.UnitTests
{
    public class CounterOrchestration : TaskOrchestration<int, TestOrchestrationInput>
    {
        public override Task<int> RunTask(OrchestrationContext context, TestOrchestrationInput input)
        {
            int cnt = input.Counter;

            while (cnt < 0)
            {
                cnt++;
                Console.WriteLine(cnt);
                Task.Delay(input.Delay);
            }

            return Task.FromResult<int>(cnt);
        }
    }
}
