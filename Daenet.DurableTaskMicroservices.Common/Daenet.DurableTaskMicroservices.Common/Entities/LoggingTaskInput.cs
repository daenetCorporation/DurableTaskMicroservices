//  ----------------------------------------------------------------------------------
//  Copyright daenet Gesellschaft für Informationstechnologie mbH
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  ----------------------------------------------------------------------------------
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
