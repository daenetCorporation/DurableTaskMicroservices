using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    /// <summary>
    /// Defines the task configuration.
    /// </summary>
    [DataContract]
    public class TaskConfig
    {
      
        /// <summary>
        /// The Trase Source Name used by logging.
        /// If you don't set it it is replaced by LogTraceSourceName of the parent orchestration.
        /// </summary>
        [DataMember]
        public string LogTraceSourceName { get; set; }

        /// <summary>
        /// Holds the list of all validator rules which have to be executed in a chain.
        /// The chain of rules will be executed before the task is executed.
        /// If any of rules does not pass validation (returns FALSE) the task will not be executed.
        /// </summary>
        [DataMember]
        public ICollection<ValidationRuleDescriptor> ValidatorRules { get; set; }
    }
}
