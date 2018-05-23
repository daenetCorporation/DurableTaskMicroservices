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
//  ----------------------------------------------------------------------------------using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Rules
{

    /// <summary>
    /// Defines the result of execution of a routing rule. <see cref="RoutingRule"/>
    /// </summary>
    public class RoutingRuleResult
    {
        /// <summary>
        /// Name of the System which was chosen from the routing rule
        /// </summary>
        public string SystemName{ get; set; }

        /// <summary>
        /// Exception of the Routed event
        /// If Exception is null, no error occured
        /// </summary>
        public string Exception { get; set; }
    }
}
