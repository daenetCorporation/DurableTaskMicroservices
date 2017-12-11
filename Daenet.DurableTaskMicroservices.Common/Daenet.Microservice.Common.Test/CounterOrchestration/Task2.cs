using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using Daenet.DurableTaskMicroservices.Common.Entities;
using DurableTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Daenet.DurableTaskMicroservices.UnitTests
{
    public class Task2 : TaskBase<TaskInput, Null>
    {
        protected override Null RunTask(TaskContext context, TaskInput input)
        {
            Debug.WriteLine($"Executing Task {nameof(Task2)}. Data: {input.Data}");
            return new Null();
        }
    }
}
