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
using System.Runtime.Serialization;
using System.Text;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    /// <summary>
    /// Generic implementation of TaskInput class, where the Data property get's type information via type param
    /// </summary>
    [DataContract]
    public class TaskInput<T> : TaskInput
    {
        /// <summary>
        /// Task input argument. This is typically set programmatically by orchestration.
        /// You are not required to set this in configuration of the task.
        /// </summary>
        [DataMember]
        public new T Data { get; set; }
    }
}
