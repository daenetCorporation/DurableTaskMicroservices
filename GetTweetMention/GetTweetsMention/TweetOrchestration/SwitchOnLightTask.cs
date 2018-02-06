using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using DurableTask.Core;
using Iot;
using Microsoft.Extensions.Logging;
using System;
using PhilipsHueConnector;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhilipsHueConnector.Entities;

namespace GetTweetsMention
{
    public class SwitchOnLightTask : TaskBase<SwitchOnLightTaskInput, Null>
    {
        private SwitchOnLightTaskInput m_Input;

        protected override Null RunTask(TaskContext context, SwitchOnLightTaskInput input, ILogger logger)
        {
            logger.LogInformation($"{nameof(SwitchOnLightTask)} entered.");
            this.m_Input = input;

            if (input.OldTweetId != input.LatestTweetId)
            {
                logger.LogInformation("New mention got in tweets.");

                SwitchOnLight();

                Task.Delay(5000).Wait();

                SwitchOffLight();
            }

            logger.LogInformation($"{nameof(SwitchOnLightTask)} exit.");

            return new Null();
        }

        /// <summary>
        /// Switch On the light
        /// </summary>
        public void SwitchOnLight()
        {
            var iotApi = getIotApi();

            var result = iotApi.SendAsync(new SetLightStates()
            {
                Id = m_Input.DeviceId,

                Body = new State()
                {
                    on = true,
                    bri = 120
                },

            }).Result;
        }

        /// <summary>
        /// Switch Off the light
        /// </summary>
        public void SwitchOffLight()
        {
            var iotApi = getIotApi();

            var result = iotApi.SendAsync(new SetLightStates()
            {
                Id = m_Input.DeviceId,

                Body = new State()
                {
                    on = false
                },

            }).Result;
        }

        /// <summary>
        /// Get IotApi
        /// </summary>
        /// <returns></returns>
        private IotApi getIotApi()
        {
            var api = new IotApi();
            api.UsePhilpsQueueRest(m_Input.GatewayUrl, m_Input.UserName);
            api.Open();
            return api;
        }
    }
}
