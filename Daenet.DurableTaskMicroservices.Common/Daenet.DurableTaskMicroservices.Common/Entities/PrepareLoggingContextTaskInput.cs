using Daenet.Diagnostics;
using DurableTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    [DataContract]
    public class PrepareLoggingContextTaskInput : TaskInput
    {
        [DataMember]
        public LoggingContext ParentLoggingContext { get; set; }

        //public string LogTraceSourceName { get; set; }

        [DataMember]
        public OrchestrationContext ParentOrchestrationContext { get; set; }

        [DataMember]
        public OrchestrationInput ParentOrchestrationInput { get; set; }

        /// <summary>
        /// Orchestration LogTraceSource.
        /// </summary>
        [DataMember]
        public string LogTraceSourceName { get; set; }

    }

}
