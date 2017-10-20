using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    [DataContract]
    public class OrchestrationInput
    {
        /// <summary>
        /// initializes Context dictionary
        /// </summary>
        public OrchestrationInput()
        {
            Context = new Dictionary<string, object>();
        }

        /// <summary>
        /// Dictionary of values which defines context.
        /// </summary>
        [DataMember]
        public Dictionary<string, object> Context { get; set; }

        [DataMember]
        public ILoggerFactory LoggerFactory { get; set; }

        /// <summary>
        /// This property indicates that the internal logging context was already 
        /// initializad (propagated).
        /// </summary>
        public bool IsInitialized { get; set; }

        /// <summary>
        /// Orchestration Input Argument.
        /// </summary>
        public object InputArg { get; set; }

    }
}
