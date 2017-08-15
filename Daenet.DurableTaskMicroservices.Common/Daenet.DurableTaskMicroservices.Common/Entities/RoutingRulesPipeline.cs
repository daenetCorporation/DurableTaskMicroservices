using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    /// <summary>
    /// Used as ParentOrchestrationInput argument of ValidationRulesTask.
    /// </summary>
    public class RoutingRulesPipeline
    {
        /// <summary>
        /// Holds the instance of the entity which has to be conditioned for routing.
        /// </summary>
        public object EntityInstance { get; set; }

        /// <summary>
        /// Holds the list of all rules which have to be executed in a chain.
        /// </summary>
        public ICollection<RoutingRuleDescriptor> Rules { get; set; }
    }
}
