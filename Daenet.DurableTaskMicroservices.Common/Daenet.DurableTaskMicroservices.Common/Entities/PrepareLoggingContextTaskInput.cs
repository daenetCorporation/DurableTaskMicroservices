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
