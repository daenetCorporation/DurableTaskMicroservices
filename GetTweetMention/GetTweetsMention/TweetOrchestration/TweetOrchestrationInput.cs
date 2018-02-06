using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Daenet.DurableTaskMicroservices.Common.Entities;

namespace GetTweetsMention
{
   // [KnownTypeAttribute(typeof(OrchestrationInput))]
    [DataContractAttribute]
    public class TweetOrchestrationInput : OrchestrationInput
    {
        [DataMember]
        public string LatestTweetId { get; set; }
    }
}
