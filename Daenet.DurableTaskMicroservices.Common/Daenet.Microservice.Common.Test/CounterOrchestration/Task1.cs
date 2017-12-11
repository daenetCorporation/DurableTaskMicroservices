using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using Daenet.DurableTaskMicroservices.Common.Entities;
using DurableTask;
using System;
using System.Collections.Generic;
using DurableTask.Core;
using System.Diagnostics;

namespace Daenet.DurableTaskMicroservices.UnitTests
{
    public class Task1 : TaskBase<TaskInput, Null>
    {
        protected override Null RunTask(TaskContext context, TaskInput input)
        {
            Debug.WriteLine($"Executing Task {nameof(Task1)}. Data: {input.Data}");
            return new Null();
        }
    }
}
