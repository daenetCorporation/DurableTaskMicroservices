using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    /// <summary>
    /// Describes the rule with name and assembly qualified name. 
    /// </summary>
    public class RoutingRuleDescriptor : RuleDescriptor
    {
        /// <summary>
        /// The instance of the routing rule. If this value is not set then <see cref="RuleQualifiedName"/> must be set.
        /// </summary>
        internal RoutingRule RoutingRule{ get; set; }
    }
}
