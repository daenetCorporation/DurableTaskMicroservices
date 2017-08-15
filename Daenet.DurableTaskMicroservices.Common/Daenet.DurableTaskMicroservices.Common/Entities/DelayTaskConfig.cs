using Daenet.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    /// <summary>
    /// Defines the configuration of DelayTask.
    /// </summary>
    [DataContract]
    public class DelayTaskConfig : TaskConfig
    {
        /// <summary>
        /// Delay interval of task specified in seconds.
        /// </summary>
       [DataMember]
        public int DelayIntervalInSeconds { get; set; }


    }

}
