using System;
using System.Collections.Generic;
using System.Text;

namespace Daenet.DurableTaskMicroservices
{
    /// <summary>
    /// Put this attribute in assembly which implements integration Microservices.
    /// It is required by host loader to load all integration relevant classes from this assembly.
    /// If you don't put this attribute, no any type can be used in serialization process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class IntegrationAssemblyAttribute : Attribute
    {

    }
}
