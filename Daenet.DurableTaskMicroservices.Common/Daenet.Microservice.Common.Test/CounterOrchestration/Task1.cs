using Daenet.DurableTask.Microservices;
using DurableTask.Core;
using System.Diagnostics;

namespace Daenet.DurableTaskMicroservices.UnitTests
{
    public class Task1 : TaskActivity<string, Null>
    {
        protected override Null Execute(TaskContext context, string input)
        {
            Debug.WriteLine($"Executing Task {nameof(Task1)}");
            return new Null();
        }
    }
}
