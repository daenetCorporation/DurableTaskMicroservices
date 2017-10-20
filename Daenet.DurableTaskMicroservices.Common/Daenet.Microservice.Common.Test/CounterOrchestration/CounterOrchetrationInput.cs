
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

namespace Daenet.DurableTaskMicroservices.UnitTests
{
    [DataContractAttribute]
    public class CounterOrchetrationInput
    {
        [DataMember]
        public int Counter { get; set; }

        [DataMember]
        public int Delay { get; set; }
    }
}
