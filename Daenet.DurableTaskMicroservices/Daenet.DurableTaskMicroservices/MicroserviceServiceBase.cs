using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Daenet.DurableTaskMicroservices.Common.Entities;
using DurableTask;
using DurableTask.Core;

namespace Daenet.DurableTask.Microservices
{
    /// <summary>
    /// Base class for common Service and Client functionality. 
    /// </summary>
    public class MicroserviceBase
    {
        protected TaskHubClient m_HubClient;
         
        public const string cActivityIdCtxName = "ActivityId";

        #region Public Methods
        /// <summary>
        /// Starts the new instance of the microservice by passing input arguments.
        /// This method will start the new instance of orchestration
        /// </summary>
        /// <param name="orchestration">The type of orchestration to be started.</param>
        /// <param name="inputArgs">Input arguments.</param>
        /// <returns></returns>
        public Task<MicroserviceInstance> StartServiceAsync(Type orchestration, object inputArgs, Dictionary<string, object> context = null)
        {
            return createServiceInstanceAsync(orchestration, inputArgs, context);
        }

        #region removed
        /// <summary>
        /// Starts the new instance of the microservice by passing input arguments.
        /// This method will start the new instance of orchestration
        /// </summary>
        /// <param name="orchestrationQualifiedName">The full qualified name of orchestration to be started.</param>
        /// <param name="inputArgs">Input arguments.</param>
        /// <returns></returns>
        //public Task<MicroserviceInstance> StartServiceAsync(string orchestrationQualifiedName, object inputArgs, Dictionary<string, object> context = null)
        //{
        //    //return StartServiceAsync(Type.GetType(orchestrationQualifiedName), inputArgs, context);
        //}

        /// <summary>
        /// Creates the instance of service from service name.
        /// </summary>
        /// <param name="orchestrationQualifiedNameOrName"></param>
        /// <param name="inputArgs"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        //protected Task<MicroserviceInstance> CreateServiceInstanceAsync(string orchestrationQualifiedName, object inputArgs, Dictionary<string, object> context)
        //{
        //    return createServiceInstanceAsync(Type.GetType(orchestrationQualifiedName), inputArgs, context);
        //}
        #endregion

        /// <summary>
        /// Starts the new instance of the microservice by passing input arguments.
        /// This method will start the new instance of orchestration
        /// </summary>
        /// <param name="orchestrationQualifiedName">The full qualified name of orchestration to be started.</param>
        /// <param name="inputArgs">Input arguments.</param>
        /// <returns></returns>
        public async Task<MicroserviceInstance> StartServiceAsync(string orchestrationQualifiedNameOrName, object inputArgs, Dictionary<string, object> context = null, string version = "")
        {
            try
            {
                var tokens = orchestrationQualifiedNameOrName.Split(',');
                if (tokens.Length > 1)
                    orchestrationQualifiedNameOrName = tokens[0];

                ensureActIdInContext(context, inputArgs);

                var ms = new MicroserviceInstance()
                {
                    OrchestrationInstance = await m_HubClient.CreateOrchestrationInstanceAsync(orchestrationQualifiedNameOrName, version, inputArgs),
                };
                return ms;
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == "StorageException" && ex.Message.Contains(": (404) Not Found"))
                {
                    throw new Exception("The microservce host should be started at least once, before starting orchestration. This error may also indicate, that incorrect hub name is used.", ex);
                }
                throw;
            }
        }
        

        /// <summary>
        /// Waits on single instance to complete execution with any of supported states.
        /// Instance is running if it is in one of Pending, ContinuedAsNew or Running states.
        /// </summary>
        /// <param name="instance">The instance of the service to wait on.</param>
        /// <param name="wait"></param>
        /// <returns>Returns the state of orchestration after it has enetered on of terminal states: 
        /// Canceled, Termnated, Failed or Completed.</returns>
        public async Task<OrchestrationState> WaitOnInstanceAsync(MicroserviceInstance instance, int wait = int.MaxValue)
        {
            var state = await this.m_HubClient.WaitForOrchestrationAsync(instance.OrchestrationInstance, TimeSpan.FromMilliseconds(wait));
            return state;
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Here we create a context and ensure that ActivityId is provided.
        /// </summary>
        /// <param name="orchestration"></param>
        /// <param name="inputArgs"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<MicroserviceInstance> createServiceInstanceAsync(Type orchestration, object inputArgs, Dictionary<string, object> context, string version = null)
        {
            ensureActIdInContext(context, inputArgs);

            var ms = new MicroserviceInstance()
            {
                OrchestrationInstance = await m_HubClient.CreateOrchestrationInstanceAsync(orchestration, inputArgs),
            };
            return ms;
        }

        private static void ensureActIdInContext(Dictionary<string, object> context, object inputArgs)
        {
            OrchestrationInput orchestrationInput = inputArgs as OrchestrationInput;

            if (orchestrationInput != null)
            {
                if (orchestrationInput.Context == null)
                    orchestrationInput.Context = new Dictionary<string, object>(context);

                if (orchestrationInput.Context.ContainsKey(cActivityIdCtxName) == false)
                {
                    orchestrationInput.Context.Add(cActivityIdCtxName, Guid.NewGuid().ToString());
                }
            }
        }
        #endregion

    }
}
