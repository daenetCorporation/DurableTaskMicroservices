using Daenet.DurableTaskMicroservices.Common.Entities;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;


namespace Daenet.DurableTaskMicroservices.Common.BaseClasses
{
    public abstract class SendAdapterBase<TInput, TAdapterOutput> : TaskBase<TInput, TAdapterOutput>
        where TInput : TaskInput
        where TAdapterOutput : class
    {

        protected override TAdapterOutput RunTask(TaskContext context, TInput input, ILogger logger)
        {
            try
            {
                var rcvData = SendData(context, input, logger);

                return rcvData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected abstract TAdapterOutput SendData(TaskContext context, TInput input, ILogger logger);

    }
}
