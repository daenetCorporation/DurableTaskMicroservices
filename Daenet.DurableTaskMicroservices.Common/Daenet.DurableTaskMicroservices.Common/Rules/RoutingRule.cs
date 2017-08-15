using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Rules
{
    /// <summary>
    /// Defines the rule which calcuilates the next executing task in dependence on
    /// rule conditions.
    /// </summary>
    public abstract class RoutingRule : RuleBase
    {
        /// <summary>
        /// Rules with higher priority are first executed.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets the type of the next task which has to be executed depending on internal rule's condition.
        /// </summary>
        /// <param name="entityInstance">The instance of the rule.</param>
        /// <returns>The lists of tasks task which will be scheduled as next in parallel. If the rule does not satisfy routing condition, the
        /// empty list od task-types should be returned.</returns>
        public abstract ICollection<Type> GetNextTasks(object entityInstance);
        
        /// <summary>
        /// Do not invoke this method directly!
        /// </summary>
        /// <param name="entityInstance"></param>
        /// <returns></returns>
        public override object Execute(object entityInstance)
        {
            return GetNextTasks(entityInstance);    
        }
    }
}
