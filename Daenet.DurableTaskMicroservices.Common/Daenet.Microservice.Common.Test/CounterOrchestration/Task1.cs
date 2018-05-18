using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using Daenet.DurableTaskMicroservices.Common.Entities;
using DurableTask;
using System;
using System.Collections.Generic;
using DurableTask.Core;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Daenet.Microservice.Common.Test.CounterOrchestration;

namespace Daenet.DurableTaskMicroservices.Common.Test.CounterOrchestration
{
    public class Task1 : TaskBase<Task1Input, Null>
    {
  
        protected override Null RunTask(TaskContext context, Task1Input input, ILogger logger)
        {
            Debug.WriteLine($"Executing Task {nameof(Task1)}. Input Text: {input.Text}");
            logger.LogInformation($"Executing Task {nameof(Task1)}. Input: {input.Text}");
            return new Null();
        }
    }
}
