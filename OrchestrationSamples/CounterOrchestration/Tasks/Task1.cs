using Daenet.DurableTask.Microservices;
using DurableTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CounterOrchestration.Tasks
{
    public class Task1 : TaskActivity<string, string>
    {
        protected override string Execute(TaskContext context, string input)
        {
            Debug.WriteLine($"Executing Task {nameof(Task1)}");
            return null;
        }
    }
}
