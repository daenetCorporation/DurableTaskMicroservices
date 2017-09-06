using Daenet.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    [DataContract]
    public class LoggingTaskInput : TaskInput
    {
       [DataMember]
        public LoggingContext LoggingContext { get; set; }

       [DataMember]
       public Action LoggingAction { get; set; }

       [DataMember]
       public TracingLevel TracingLevel { get; set; }

       [DataMember]
        public int EventId { get; set; }

       [DataMember]
       public string FormatedMessage { get; set; }

       [DataMember]
       public object[] MessageParams { get; set; }

       [DataMember]
       public Exception Exception { get; set; }

        /// <summary>
        /// The Trase Source Name used by logging.
        /// </summary>
        [DataMember]
        public string LogTraceSourceName { get; set; }

    }

    public enum Action
    {
        TraceInfo,
        TraceWarning,
        TraceError
    }
}
