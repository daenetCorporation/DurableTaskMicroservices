using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    /// <summary>
    /// Configuration for FileSendAdapter.
    /// </summary>
    [DataContract]
    public class FileSendAdapterConfig : SqlSendAdapterConfig
    {
        /// <summary>
        /// The name of the file. You can use PropertyName to generate different file names in dependence 
        /// on the instance.
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string QueueFolder { get; set; }

        [DataMember]
        public string MapperQualifiedName { get; set; }

        /// <summary>
        /// Specifies the name of property of the Instance instance which will be 
        /// used in string formatting of the file name.
        /// For example: FileName = "Hello {0}" and PropertyName = 'Time' will be rendered as
        /// String.Format(FileName, Instance.Time).
        /// </summary>
        [DataMember]
        public string PropertyName { get; set; }

    }
}
