using Daenet.Diagnostics;
using Daenet.System.Integration.Entities;
using DurableTask;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Daenet.System.Integration.Extensions;


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
            catch(Exception ex)
            {
                throw;
            }          
        }

        protected abstract TAdapterOutput SendData(TaskContext context, TInput input);        

    }
}
