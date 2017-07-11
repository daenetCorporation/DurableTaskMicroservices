using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTask.Microservices
{
    public interface IConfig
    {
        string InstanceName { get; }

       // Type Type{ get; }
    }
}
