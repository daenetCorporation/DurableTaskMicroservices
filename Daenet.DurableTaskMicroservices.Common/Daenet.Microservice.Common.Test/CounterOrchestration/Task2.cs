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
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using Daenet.DurableTaskMicroservices.Core;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Daenet.DurableTaskMicroservices.Common.Test.CounterOrchestration
{
    public class Task2 : TaskBase<Task2Input, Null>
    {
        protected override Null RunTask(TaskContext context, Task2Input input, ILogger logger)
        {
            Debug.WriteLine($"Executing Task {nameof(Task2)}. Number: {input.Number}");
            logger.LogInformation($"Executing Task {nameof(Task2)}. Number: {input.Number}");
            return new Null();
        }
    }
}
