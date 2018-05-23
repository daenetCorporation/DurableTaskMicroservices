//  ----------------------------------------------------------------------------------
//  Copyright daenet Gesellschaft für Informationstechnologie mbH
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  ----------------------------------------------------------------------------------
using Daenet.DurableTaskMicroservices.Common;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using Daenet.DurableTaskMicroservices.Common.Entities;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Daenet.DurableTaskMicroservices.Common.Adapters
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
