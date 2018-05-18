using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using Daenet.DurableTaskMicroservices.Common.Entities;
using Daenet.Microservice.Common.Test.CounterOrchestration;
using DurableTask;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Daenet.DurableTaskMicroservices.Common.Test.CounterOrchestration
{
    public class Task2 : TaskBase<Task2Input, Null>
    {
        protected override Null RunTask(TaskContext context, Task2Input input, ILogger logger)
        {
            Debug.WriteLine($"Executing Task {nameof(Task2)}. Number: {input.Number}");
            logger.LogInformation($"Executing Task {nameof(Task2)}. Number: {input.Number}");
            return new Null();
        }
    }
}
