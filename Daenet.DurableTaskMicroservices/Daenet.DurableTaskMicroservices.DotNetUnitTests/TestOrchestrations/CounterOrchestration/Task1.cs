using Daenet.DurableTask.Microservices;
using DurableTask;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daenet.DurableTaskMicroservices.UnitTests
{
    public class Task1 : TaskActivity<string, Null>
    {
        protected override Null Execute(TaskContext context, string input)
        {
            Console.WriteLine($"Executing Task {nameof(Task1)}");
            return new Null();
        }
    }
}
