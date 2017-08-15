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
