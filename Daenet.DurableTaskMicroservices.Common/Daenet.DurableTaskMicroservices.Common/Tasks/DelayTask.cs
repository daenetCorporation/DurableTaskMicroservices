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
using DurableTask.Core;
using System.Threading;

namespace Daenet.DurableTaskMicroservices.Common.Tasks
{
    /// <summary>
    /// Task for waiting
    /// </summary>
    public class DelayTask : TaskActivity<int, Null>
    {
        protected override Null Execute(TaskContext context, int interval)
        {
            // configuration for this Task is in seconds, so multiply with 1000
            interval = interval * 1000;
            Thread.Sleep(interval);
            return new Null();
        }
    }
}
