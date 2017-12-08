using Daenet.DurableTaskMicroservices.Common.Entities;
using DurableTask.Core;
using System;


namespace Daenet.DurableTaskMicroservices.Common.BaseClasses
{
    public abstract class SendAdapterBase<TInput, TAdapterOutput> : TaskBase<TInput, TAdapterOutput>
        where TInput : TaskInput
        where TAdapterOutput : class
    {

        protected override TAdapterOutput RunTask(TaskContext context, TInput input)
        {
            try
            {
                var rcvData = SendData(context, input);

                return rcvData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected abstract TAdapterOutput SendData(TaskContext context, TInput input);

    }
}
