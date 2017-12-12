using Daenet.DurableTaskMicroservices.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.Microservice.Common.Test.CounterOrchestration
{
    public class Task1Input : TaskInput
    {
        public string Text { get; set; }
    }
}
