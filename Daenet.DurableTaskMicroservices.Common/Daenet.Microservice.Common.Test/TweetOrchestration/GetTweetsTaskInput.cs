using Daenet.DurableTaskMicroservices.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.Microservice.Common.Test
{
    public class GetTweetsTaskInput : TaskInput
    {
        /// <summary>
        /// Twitter Consumer Key
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Twitter Consumer secret 
        /// </summary>
        [DataMember]
        public string Secret { get; set; }

        /// <summary>
        /// Twitter User Name 
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Amount of Tweets
        /// </summary>
        [DataMember]
        public int Count { get; set; }
    }
}
