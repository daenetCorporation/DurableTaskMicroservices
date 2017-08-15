using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Rules
{

    /// <summary>
    /// Defines the result of execution of a routing rule. <see cref="RoutingRule"/>
    /// </summary>
    public class RoutingRuleResult
    {
        /// <summary>
        /// Name of the System which was chosen from the routing rule
        /// </summary>
        public string SystemName{ get; set; }

        /// <summary>
        /// Exception of the Routed event
        /// If Exception is null, no error occured
        /// </summary>
        public string Exception { get; set; }
    }
}
