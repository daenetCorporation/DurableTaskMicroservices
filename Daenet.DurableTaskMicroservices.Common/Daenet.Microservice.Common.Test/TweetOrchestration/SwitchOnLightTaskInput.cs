using Daenet.DurableTaskMicroservices.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.Microservice.Common.Test
{
    public class SwitchOnLightTaskInput : TaskInput
    {
        /// <summary>
        /// Philips hue gateway url
        /// </summary>
        [DataMember]
        public string GatewayUrl { get; set; }

        /// <summary>
        /// Philips hue gateway user name 
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Connected with Philips hue gateway Device Id 
        /// </summary>
        [DataMember]
        public string DeviceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string OldTweetId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string LatestTweetId { get; set; }
    }
}
