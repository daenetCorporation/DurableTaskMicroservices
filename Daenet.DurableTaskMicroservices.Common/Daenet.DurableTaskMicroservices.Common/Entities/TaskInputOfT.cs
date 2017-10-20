using System;
using System.Collections.Generic;
using System.Text;

namespace Daenet.DurableTaskMicroservices.Common.Entities
{
    /// <summary>
    /// Generic implementation of TaskInput class, where the Data property get's type information via type param
    /// </summary>
    [DataContract]
    public class TaskInput<T> : TaskInput
    {
        /// <summary>
        /// Task input argument. This is typically set programmatically by orchestration.
        /// You are not required to set this in configuration of the task.
        /// </summary>
        [DataMember]
        public new T Data { get; set; }
    }
}
