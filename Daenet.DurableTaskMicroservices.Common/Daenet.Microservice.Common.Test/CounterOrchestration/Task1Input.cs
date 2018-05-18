using Daenet.DurableTaskMicroservices.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Test.CounterOrchestration
{
    public class Task1Input : TaskInput
    {
        [DataMember]
        public string Text { get; set; }
    }
}
