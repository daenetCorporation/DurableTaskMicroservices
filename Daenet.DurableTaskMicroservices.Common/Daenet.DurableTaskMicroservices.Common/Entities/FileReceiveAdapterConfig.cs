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
    /// Implements configuration (input argument) for <see cref="FileReceiveAdapter{TAdapterOutput}" />.
    /// </summary>
    [DataContract]
    public class FileReceiveAdapterConfig : TaskConfig
    {
        /// <summary>
        /// File mask used for getting of files inspecified folder.
        /// </summary>
        [DataMember]
        public string FileMask { get; set; }

        /// <summary>
        /// The folder which is used as input file queue.
        /// </summary>
        [DataMember]
        public string QueueFolder { get; set; }

        /// <summary>
        /// Assembly Qualified Name of the class, which implements <see cref="IAdapterMapper"/> interface.
        /// This component is responsable to map data contained in the file to instance of type which is
        /// output argument of FileReceiveAdapter task.
        /// </summary>
        [DataMember]
        public string MapperQualifiedName { get; set; }
    }
}
