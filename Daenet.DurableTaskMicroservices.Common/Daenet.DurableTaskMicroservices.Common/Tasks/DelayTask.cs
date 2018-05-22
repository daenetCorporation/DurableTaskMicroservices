using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Core;
using DurableTask.Core;
using System.Threading;

namespace Daenet.DurableTaskMicroservices.Common.Tasks
{
    /// <summary>
    /// Task for waiting
    /// </summary>
    public class DelayTask : TaskActivity<int, Null>
    {
        protected override Null Execute(TaskContext context, int interval)
        {
            // configuration for this Task is in seconds, so multiply with 1000
            interval = interval * 1000;
            Thread.Sleep(interval);
            return new Null();
        }
    }
}
