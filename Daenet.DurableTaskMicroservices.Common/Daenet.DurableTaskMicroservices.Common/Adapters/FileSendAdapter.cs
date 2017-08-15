using Daenet.Diagnostics;
using Daenet.System.Integration.Entities;
using DurableTask;
using DurableTask.Microservices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.System.Integration
{
    /// <summary>
    /// Creates a single file in queue folder and invoke sthe mapper which maps the instance to a single text line.
    /// </summary>
    /// <typeparam name="TFileSendAdapterInput"></typeparam>
    public class FileSendAdapter<TFileSendAdapterInput> : SendAdapterBase<TFileSendAdapterInput, Null>
        where TFileSendAdapterInput : FileSendAdapterInput
    {
        /// <summary>
        /// Executes sending of data to folder. This adapter creates a single file for every 'input'.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override Null SendData(DurableTask.TaskContext context, TFileSendAdapterInput input)
        {
            try
            {
                var config = this.GetConfiguration<FileSendAdapterConfig>(input.Orchestration);
                  if (!Directory.Exists(config.QueueFolder))
                    throw new Exception("No receive folder!");

                  dynamic dynInput = config;

                  string fileName = Path.Combine(config.QueueFolder, Guid.NewGuid().ToString() + "_" + config.FileName);

                  fileName = String.Format(fileName, getPropertyValue(input));

                  using (StreamWriter sw = new StreamWriter(fileName))
                  {
                      var mapper = Factory.GetAdapterMapper(config.MapperQualifiedName);

                      object line = mapper.Map(config);

                      sw.WriteLine(line as string);
                  }
            }
            catch (Exception ex)
            {
                throw;
            }

            return new Null();
        }


        /// <summary>
        /// Gets the value of the property by using reflection.
        /// </summary>
        /// <param name="input">The input instance of the task.</param>
        /// will be used for formatting of the file name.</param>
        /// <returns></returns>
        private object getPropertyValue(FileSendAdapterInput input)
        {
            var cfg = this.GetConfiguration<FileSendAdapterConfig>(input.Orchestration);

            string propname = null;
            if (cfg != null)
                propname = cfg.PropertyName;

            if (input == null)
                return String.Empty;

            if (input.Data == null)
                return String.Empty;

            if (propname == null)
                return String.Empty;

            var prop = input.GetType().GetProperty(propname, global::System.Reflection.BindingFlags.Instance | global::System.Reflection.BindingFlags.Public);
            if (prop == null)
                return String.Empty;

            return prop.GetValue(input.Data);
        }
    }
}
