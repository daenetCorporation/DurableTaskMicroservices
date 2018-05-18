using Daenet.DurableTaskMicroservices.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.Microservice.Common.Test.HelloWorldOrchestration
{
    [DataContract]
    public class HelloWorldOrchestrationInput : OrchestrationInput
    {
        [DataMember]
        public string HelloText { get; set; }
    }
}
