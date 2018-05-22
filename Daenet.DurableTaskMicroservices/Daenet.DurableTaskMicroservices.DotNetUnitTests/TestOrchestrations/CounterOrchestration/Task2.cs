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


using Daenet.DurableTaskMicroservices.Core;
using DurableTask;
using DurableTask.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Daenet.DurableTaskMicroservices.Tests.TaskOrchestration.CounterOrchestration
{
    public class Task2 : TaskActivity<string, Null>
    {
        protected override Null Execute(TaskContext context, string input)
        {
            Debug.WriteLine($"Executing Task: {nameof(Task2)}. Input: {input}");
            return new Null();
        }
    }
}
