
using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.Entities;
using DurableTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Test.CounterOrchestration
{
    [DataContractAttribute]
    public class CounterOrchestrationInput : OrchestrationInput
    {
        [DataMember]
        public int Counter { get; set; }

        [DataMember]
        public int Delay { get; set; }
    }
}
