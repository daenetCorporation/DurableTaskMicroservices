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
using Daenet.DurableTaskMicroservices.Common.Rules;
using DurableTask.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common
{
    public class RoutingManager{

        private OrchestrationContext m_Context;
        public RoutingManager(OrchestrationContext context)
        {
            m_Context = context;
        }
    

        /// <summary>
        /// Schedules list of defined routed tasks calculated from list of specified routing rules.
        /// </summary>
        /// <param name="rulesPipeline"></param>
        /// <returns></returns>
        public Task<RoutingRuleResult>[] ScheduleRoutedTasks(TaskInput taskArgument, 
            RoutingRulesPipeline rulesPipeline)
        {
            List<Task<RoutingRuleResult>> tasks = new List<Task<RoutingRuleResult>>();

            if (rulesPipeline == null || rulesPipeline.Rules == null)
                return tasks.ToArray();

            // We set the instance which will be checked by rule.
            // Not every rule can process every instance.
            rulesPipeline.EntityInstance = taskArgument;

            var routingTasks = RulesManager.GetRoutingTasks(rulesPipeline);

            foreach (var routingTaskType in routingTasks)
            {
                tasks.Add(m_Context.ScheduleTask<RoutingRuleResult>(routingTaskType, taskArgument));                
            }

            return tasks.ToArray();
        }

        //private TaskInput getMatchingParameter(Type taskType, ICollection<TaskInput> taskArgument)
        //{
        //    var runTaskMethod = taskType.GetMethod("RunTask", global::System.Reflection.BindingFlags.Instance | global::System.Reflection.BindingFlags.NonPublic);
        //    var args = runTaskMethod.GetParameters();
        //    if (args.Length == 2)
        //    {
        //        foreach (var item in taskArguments)
        //        {
        //            if (args[1].ParameterType == item.GetType())
        //                return item;
        //        }

        //        return null;
        //    }
        //    else
        //        return null;
        //}
    }
}
