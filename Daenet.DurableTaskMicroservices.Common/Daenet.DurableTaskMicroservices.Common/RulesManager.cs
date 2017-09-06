using Daenet.DurableTaskMicroservices.Common.Entities;
using Daenet.DurableTaskMicroservices.Common.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common
{
    /// <summary>
    /// Executes all defined validation rules.
    /// </summary>
    internal class RulesManager
    {
        /// <summary>
        /// Executes the chain of validation rules specified in 'input.Rules'.
        /// </summary>
         /// <param name="pipeline">Chain of validation rules.</param>
        /// <returns>Holds results of execution of all rules. If some of rules have not been executed, they 
        /// are not added in the result list.</returns>
        internal static ValidatorRulesResult RunPipeline(ValidationRulesPipeline pipeline)
        {
            ValidatorRulesResult result = new ValidatorRulesResult();

            foreach (var rule in pipeline.Rules)
            {
                if(rule.ValidatorRule == null)
                {
                    rule.ValidatorRule = Factory.GetRule(rule.RuleQName) as ValidatorRule;
                }

                if(rule.ValidatorRule.CanExecute(pipeline.EntityInstance))
                {
                    bool validationResult = rule.ValidatorRule.Validate(pipeline.EntityInstance);

                    result.Results.Add(rule.Name, validationResult);
                }
            }

            return result;
        }


        /// <summary>
        /// Executes the chain of routing rules specified in 'input.Rules'.
        /// </summary>
        /// <param name="pipeline">Chain of routing rules.</param>
        /// <returns>List of Tasks which should be scheduled.</returns>
        internal static List<Type> GetRoutingTasks(RoutingRulesPipeline pipeline)
        {
            List<Type> ruleTypes = new List<Type>();

            foreach (var rule in pipeline.Rules)
            {
                if (rule.RoutingRule == null)
                {
                    rule.RoutingRule = Factory.GetRule(rule.RuleQName) as RoutingRule;
                }

                if (rule.RoutingRule.CanExecute(pipeline.EntityInstance))
                {
                    ruleTypes.AddRange(rule.RoutingRule.GetNextTasks(pipeline.EntityInstance));                    
                }
            }

            return ruleTypes;
        }
    }
}
