using Daenet.DurableTaskMicroservices.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.Microservice.Common.Test
{
    [DataContractAttribute]
    public class TweetOrchestrationInput : OrchestrationInput
    {
        [DataMember]
        public string LatestTweetId { get; set; }
    }
}
