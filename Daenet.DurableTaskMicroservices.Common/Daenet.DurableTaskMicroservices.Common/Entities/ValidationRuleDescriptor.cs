using Daenet.DurableTaskMicroservices.Common.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    /// <summary>
    /// Describes the rule with name and assembly qualified name. 
    /// </summary>
    [DataContract]
    public class ValidationRuleDescriptor : RuleDescriptor
    {
      
        /// <summary>
        /// The instance of validator. If this value is not set then <see cref="RuleQualifiedName"/> must be set.
        /// </summary>
        [DataMember]
        internal ValidatorRule ValidatorRule{ get; set; }
    }
}
