using Daenet.DurableTaskMicroservices.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GetTweetsMention
{
    public class GetTweetsTaskInput : TaskInput
    {
        /// <summary>
        /// Twitter Consumer Key
        /// </summary>
        [DataMember]
        public string ConsumerKey { get; set; }

        /// <summary>
        /// Twitter Consumer secret 
        /// </summary>
        [DataMember]
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string AccessToken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string AccessTokenSecret { get; set; }

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
