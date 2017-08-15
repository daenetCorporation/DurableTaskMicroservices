using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    /// <summary>
    /// Used as ParentOrchestrationInput argument of ValidationRulesTask.
    /// </summary>
    [DataContract]
    internal class ValidationRulesPipeline
    {
        /// <summary>
        /// Holds the instance of the entity which has to be validated.
        /// </summary>
        [DataMember]
        public object EntityInstance { get; set; }

        /// <summary>
        /// Holds the list of all rules which have to be executed in a chain.
        /// </summary>
        [DataMember]
        public ICollection<ValidationRuleDescriptor> Rules { get; set; }
    }
}
