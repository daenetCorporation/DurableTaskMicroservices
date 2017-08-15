using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.BaseClasses
{
    public abstract class MapperBase : IAdapterMapper
    {
        public abstract object Map(object input);
        
    }
}
