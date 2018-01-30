using Daenet.DurableTaskMicroservices.Common;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using Daenet.DurableTaskMicroservices.Common.Entities;
using Daenet.DurableTaskMicroservices.Common.Logging;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Daenet.Integration
{

    public class FileReceiveAdapter<TAdapterOutput> : ReceiveAdapterBase<TaskInput, TAdapterOutput> where TAdapterOutput : class
    {
        /// <summary>
        /// Receives data from file
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override TAdapterOutput ReceiveData(TaskContext context, TaskInput input, ILogger logger)
        {
            try
            {
                FileReceiveAdapterConfig config = this.GetConfiguration<FileReceiveAdapterConfig>(input.Orchestration);

                LogManager.TraceInfStartFileReceiveAdapter(config != null ? config.FileMask : "NULL", config != null? config.QueueFolder : "NULL");

                if (!Directory.Exists(config.QueueFolder))
                    throw new Exception("No receive folder!");

                var mapper = Factory.GetAdapterMapper(config.MapperQualifiedName);

                foreach (var file in Directory.GetFiles(config.QueueFolder, config.FileMask))
                {
                    TAdapterOutput inst = (TAdapterOutput)mapper.Map(file);

                    FileInfo fi = new FileInfo(file);

                    File.Move(file, file + ".tmp");

                    LogManager.TraceInfEndFileReceiveAdapter(file);

                    return inst;
                }

                LogManager.TraceInfEndFileReceiveAdapter("No file found");
            }
            catch (Exception ex)
            {
                LogManager.TraceErrFileReceiveAdapter(ex);
                throw;
            }

            return null;
        }
    }
}
