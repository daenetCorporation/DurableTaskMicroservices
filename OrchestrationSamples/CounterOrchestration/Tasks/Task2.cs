
using DurableTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CounterOrchestration.Tasks
{
    public class Task2 : TaskActivity<string, string>
    {
        protected override string Execute(TaskContext context, string input)
        {
            Debug.WriteLine($"Executing Task {nameof(Task2)}");
            return null;
        }
    }
}
