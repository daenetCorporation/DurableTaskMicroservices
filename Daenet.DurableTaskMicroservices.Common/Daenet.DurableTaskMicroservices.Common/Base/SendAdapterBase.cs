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
using Daenet.DurableTaskMicroservices.Common.Entities;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;


namespace Daenet.DurableTaskMicroservices.Common.Base
{
    public abstract class SendAdapterBase<TInput, TAdapterOutput> : TaskBase<TInput, TAdapterOutput>
        where TInput : TaskInput
        where TAdapterOutput : class
    {

        protected override TAdapterOutput RunTask(TaskContext context, TInput input, ILogger logger)
        {
            try
            {
                var rcvData = SendData(context, input, logger);

                return rcvData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected abstract TAdapterOutput SendData(TaskContext context, TInput input, ILogger logger);

    }
}
