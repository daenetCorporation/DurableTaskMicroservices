//  ----------------------------------------------------------------------------------
//  Copyright daenet Gesellschaft für Informationstechnologie mbH
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  ----------------------------------------------------------------------------------
using Daenet.DurableTaskMicroservices.Common.Base;
using Daenet.DurableTaskMicroservices.Core;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Test.CounterOrchestration
{
    public class CounterOrchestration : OrchestrationBase<CounterOrchestrationInput, Null>
    {       
        protected override async Task<Null> RunOrchestration(OrchestrationContext context, CounterOrchestrationInput input, ILogger logger)
        {
            Debug.WriteLine($"Counter: {input.Counter}");

            if (!context.IsReplaying)
                logger.LogInformation("Orchestration started with Input: {input}.", input.Counter);

            await ScheduleTask<Null>(typeof(Task1), new Task1Input() { Text = "Text passed from Orchestration." });

            await ScheduleTask<Null>(typeof(Task2), new Task2Input() { Number = 2 });
            
            Task.Delay(100).Wait();

            input.Counter--;
            if (input.Counter > 0)
            {
                if (!context.IsReplaying)
                    logger.LogInformation("Orchestration will ContinueAsNew.");

                context.ContinueAsNew(input);
            }

            return new Null();
        }
    }
}
