using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    /// <summary>
    /// Put this attribute in assembly which implements integration microservices.
    /// It is required by host loader to load all integration relevant classes from this assembly.
    /// If you don't put this attribute, no any type can be used in serialization process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class IntegrationAssemblyAttribute : Attribute
    {

    }
}
