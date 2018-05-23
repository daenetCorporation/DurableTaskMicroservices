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
using DurableTask.Core;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Daenet.DurableTaskMicroservices.Core;

namespace Daenet.DurableTaskMicroservices.Common.Test.CounterOrchestration
{
    public class Task1 : TaskBase<Task1Input, Null>
    {
  
        protected override Null RunTask(TaskContext context, Task1Input input, ILogger logger)
        {
            Debug.WriteLine($"Executing Task {nameof(Task1)}. Input Text: {input.Text}");
            logger.LogInformation($"Executing Task {nameof(Task1)}. Input: {input.Text}");
            return new Null();
        }
    }
}
