using Daenet.Common.Logging;
using Daenet.DurableTaskMicroservices.Core;
using DurableTask.Core;
using System.Runtime.Serialization;

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
