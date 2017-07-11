using Daenet.DurableTask.Microservices;
using DurableTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Daenet.DurableTaskMicroservices.UnitTests
{
    public class Task2 : TaskActivity<string, Null>
    {
        protected override Null Execute(TaskContext context, string input)
        {
            Debug.WriteLine($"Executing Task {nameof(Task2)}");
            return new Null();
        }
    }
}
