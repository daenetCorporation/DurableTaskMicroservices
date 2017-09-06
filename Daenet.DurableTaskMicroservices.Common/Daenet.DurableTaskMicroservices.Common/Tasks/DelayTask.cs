using Daenet.DurableTask.Microservices;
using DurableTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
