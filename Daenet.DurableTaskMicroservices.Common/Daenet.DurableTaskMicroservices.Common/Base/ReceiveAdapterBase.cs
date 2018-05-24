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
using Daenet.DurableTaskMicroservices.Common.Exceptions;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;


namespace Daenet.DurableTaskMicroservices.Common.Base
{
    public abstract class ReceiveAdapterBase<TInput, TAdapterOutput> : TaskBase<TInput, TAdapterOutput>
        where TInput : TaskInput
        where TAdapterOutput : class
    {
  
       
        protected override TAdapterOutput RunTask(TaskContext context, TInput input, ILogger logger)
        {
            var rcvData = ReceiveData(context, input, logger);

            ValidatorRulesResult validationResult;
            if (executeValidationRules(rcvData, this.GetConfiguration(input.Orchestration).ValidatorRules, out validationResult))
                return rcvData;
            else
            {
                throw new ValidationRuleException(validationResult, "VALIDATION!!!", rcvData);
                //this.LogManager.TraceError(TracingLevel.Level1, 1, new ValidationException());
            }
        }

        protected abstract TAdapterOutput ReceiveData(TaskContext context, TInput input, ILogger logger);

        /// <summary>
        /// Validation Rules will be executed if they are defined.
        /// </summary>
        /// <param name="rcvData"></param>
        /// <returns></returns>
        private bool executeValidationRules(TAdapterOutput rcvData,  
            ICollection<ValidationRuleDescriptor> validatorRules,
            out ValidatorRulesResult validationResult)
        {
            validationResult = new ValidatorRulesResult();

            if (validatorRules != null)
            {
                // TODO Log.

                ValidationRulesPipeline pipeline = new ValidationRulesPipeline()
                {
                    EntityInstance = rcvData,
                    Rules = validatorRules
                };

                validationResult = RulesManager.RunPipeline(pipeline);
            }

            if ((validationResult != null && validationResult.HasFailed == false) ||
                validationResult == null)
                return true;
            else
                return false;
        }

    }
}
