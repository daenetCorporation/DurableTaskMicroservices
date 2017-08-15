using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    /// <summary>
    /// Defines the configuration of SqlReceiveAdapter
    /// </summary>
    [DataContract]
    public class SqlReceiveAdapterConfig : TaskConfig
    {
        /// <summary>
        /// Connection string.
        /// </summary>
       [DataMember]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Executes sqlCmd which checks if there is some data to fetch.
        /// </summary>
       [DataMember]
       public string CheckDataCmdText { get; set; }

        /// <summary>
        /// Executes sqlCmd which fetches the data from table.
        /// </summary>
       [DataMember]
       public string FetchDataCmdText { get; set; }

       [DataMember]
       public string MapperQualifiedName { get; set; }
    }
}
