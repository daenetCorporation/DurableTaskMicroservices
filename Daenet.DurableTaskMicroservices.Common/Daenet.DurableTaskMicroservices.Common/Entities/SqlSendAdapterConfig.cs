using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    /// <summary>
    /// Defines the configuration of SqlSendAdapter
    /// </summary>
    [DataContract]
    public class SqlSendAdapterConfig : TaskConfig
    {
        /// <summary>
        /// Connection string.
        /// </summary>
        [DataMember]
        public string ConnectionString { get; set; }

        [DataMember]
        public string MapperQualifiedName { get; set; }

    }
}
