using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.Microservice.Common.Test
{
    public class TweetOrchestration : OrchestrationBase<TweetOrchestrationInput, Null>
    {
        protected override async Task<Null> RunOrchestration(OrchestrationContext context, TweetOrchestrationInput input, ILogger logger)
        {
            var outPut = await context.ScheduleTask<GetTweetTaskOutput>(typeof(GetTweetsTask), getTweetsTaskInput());

            await context.ScheduleTask<Null>(typeof(SwitchOnLightTask), switchOnLightTaskInput(input.LatestTweetId, outPut.LatestTweetId));
            
            input.LatestTweetId = outPut.LatestTweetId;

            logger.LogInformation("Orchestration will ContinueAsNew.");
            context.ContinueAsNew(input);

            return new Null();
        }

        private GetTweetsTaskInput getTweetsTaskInput()
        {
            return new GetTweetsTaskInput()
            {
                ConsumerKey = "quXmqOcBD89bQMDH2GgejLS0r",
                ConsumerSecret = "mO6ISttJWUF8tiI6YIZ3W0Nr9NWa4KlH61th8KEPMTsPceUveD",
                AccessToken = "271351273-mGxKKELunOhYN8CvrEOS8IdQYCbR2nCByHBI4nij",
                AccessTokenSecret = "mVtaNYbeuN9UWyQIMdmEKsIb18QWVDoyD8u7Buon6OiwW",
                Name = "summoncse",
                Count = 1
            };
        }

        private SwitchOnLightTaskInput switchOnLightTaskInput(string oldId, string newId)
        {
            return new SwitchOnLightTaskInput()
            {
                GatewayUrl = "http://192.168.0.99",
                UserName = "gusp-xLeBhYznPCkz0ZQBnuZ25f3cOwRpW3tiQ8k",
                DeviceId = "4",
                OldTweetId = oldId,
                LatestTweetId = newId
            };
        }
    }
}
