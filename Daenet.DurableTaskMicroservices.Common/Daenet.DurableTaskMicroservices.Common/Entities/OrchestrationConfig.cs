using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    /// <summary>
    /// Base class for orchestration configuration.
    /// For every orchestration one configuration class should be implemented, which
    /// derives from this class.
    /// </summary>
    [DataContract]
    public class OrchestrationConfig
    {
        /// <summary>
        /// List of routing rules
        /// </summary>
        [DataMember]
        public RoutingRulesPipeline RoutingRulesPipeline { get; set; }

        /// <summary>
        /// The source name, which will explicitely override source of the parent.
        /// If this value is not set, the parent source name is used, which is common case.
        /// </summary>
        [DataMember]
        public string LogTraceSourceName { get; set; }

    }
}
