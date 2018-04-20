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
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTask.Microservices
{
    public abstract class Port<TPORTIN, TPORTOUT> : Microservice
    {
        /// <summary>
        /// Sets AutoStart to true.
        /// </summary>
        public Port()
        {
            this.AutoStart = true;
        }
        
        /// <summary>
        /// This service instance will be created when the port is triggered by message.
        /// </summary>
        public Microservice Service { get; set; }
    }
}
