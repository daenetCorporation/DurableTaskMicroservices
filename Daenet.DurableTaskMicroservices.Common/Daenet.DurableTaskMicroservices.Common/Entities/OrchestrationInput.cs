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
