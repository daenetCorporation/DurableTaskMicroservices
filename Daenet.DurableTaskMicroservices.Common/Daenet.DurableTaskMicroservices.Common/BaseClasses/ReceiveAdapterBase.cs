using Daenet.DurableTaskMicroservices.Common.Entities;
using Daenet.DurableTaskMicroservices.Common.Exceptions;
using DurableTask.Core;
using System.Collections.Generic;


namespace Daenet.DurableTaskMicroservices.Common.BaseClasses
{
    public abstract class ReceiveAdapterBase<TInput, TAdapterOutput> : TaskBase<TInput, TAdapterOutput>
        where TInput : TaskInput
        where TAdapterOutput : class
    {
  
       
        protected override TAdapterOutput RunTask(TaskContext context, TInput input)
        {
            var rcvData = ReceiveData(context, input);

            ValidatorRulesResult validationResult;
            if (executeValidationRules(rcvData, this.GetConfiguration(input.Orchestration).ValidatorRules, out validationResult))
                return rcvData;
            else
            {
                throw new ValidationRuleException(validationResult, "VALIDATION!!!", rcvData);
                //this.LogManager.TraceError(TracingLevel.Level1, 1, new ValidationException());
            }
        }

        protected abstract TAdapterOutput ReceiveData(TaskContext context, TInput input);

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
