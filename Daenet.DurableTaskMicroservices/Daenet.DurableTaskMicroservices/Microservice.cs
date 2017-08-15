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

namespace Daenet.DurableTask.Microservices
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class Microservice
    {
        /// <summary>
        /// Creates default instance of the microservice configuration.
        /// </summary>
        public Microservice()
        {
            this.ActivityConfiguration = new Dictionary<string, object>();
        }

        /// <summary>
        /// (Friendly)Name of this MicroService
        /// Used for easy identification of what happens inside this service
        /// </summary>
        public string FriendlyName { get; set; }


        /// <summary>
        /// The orchestration which hosts the service implementation.
        /// </summary>
        [DataMember]
        public string OrchestrationQName { get; set; }

        /// <summary>
        /// The orchestration which hosts the service implementation.
        /// </summary>
        [IgnoreDataMember]
        internal Type Orchestration { get; set; }

        /// <summary>
        /// List of tasks which are internally used by orchestration.
        /// </summary>
        [DataMember]
        public string[] ActivityQNames { get; set; }

        /// <summary>
        /// List of tasks which are internally used by orchestration.
        /// </summary>
        [IgnoreDataMember]
        internal Type[] Activities { get; set; }

        /// <summary>
        /// If TRUE then it will be automatically started if the instance
        /// of same orchestration is not already running. Hos ensures
        /// </summary>
        [DataMember]
        virtual public bool IsSingletone { get; set; }

        /// <summary>
        /// Dictionary of configurations of every single Task (Adapter), which is used in Microservice (orchestration).
        /// </summary>
        [DataMember]
        public Dictionary<string, object> ActivityConfiguration { get; set; }


        /// <summary>
        /// Configuration of Microservice (orchestration).
        /// </summary>
        [DataMember]
        public object ServiceConfiguration { get; set; }


        /// <summary>
        /// [Optional] Instance of the input argument of orchestration.
        /// </summary>
        [DataMember]
        public object InputArgument { get; set; }

        /// <summary>
        /// Instance of the output argument.
        /// </summary>
        [DataMember]
        public object OutputArgument { get; set; }


        public Type Type
        {
            get
            {
                return this.Orchestration;
            }
        }
    }
}
