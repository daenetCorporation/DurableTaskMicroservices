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
    public class RuleDescriptor
    {
        /// <summary>
        /// The name of the rule.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Specifies AssemblyQualified name of validator.
        /// </summary>
        [DataMember]
        public string RuleQName { get; set; }

    }
}
