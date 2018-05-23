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
