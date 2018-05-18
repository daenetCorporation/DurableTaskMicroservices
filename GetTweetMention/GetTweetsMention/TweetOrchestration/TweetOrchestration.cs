using Daenet.DurableTask.Microservices;
using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetTweetsMention
{
    public class TweetOrchestration : OrchestrationBase<TweetOrchestrationInput, Null>
    {
        protected override async Task<Null> RunOrchestration(OrchestrationContext context, TweetOrchestrationInput input, ILogger logger)
        {
            var outPut = await context.ScheduleTask<GetTweetTaskOutput>(typeof(GetTweetsTask), getTweetsTaskInput());

            await context.ScheduleTask<Null>(typeof(SwitchOnLightTask), switchOnLightTaskInput(input.LatestTweetId, outPut.LatestTweetId));
            
            input.LatestTweetId = outPut.LatestTweetId;
            
            await context.CreateTimer(context.CurrentUtcDateTime.AddMinutes(1), "");
            

            logger.LogInformation("Orchestration will ContinueAsNew.");
            context.ContinueAsNew(input);

            return new Null();
        }

        private GetTweetsTaskInput getTweetsTaskInput()
        {
            return new GetTweetsTaskInput()
            {
                ConsumerKey = "",
                ConsumerSecret = "",
                AccessToken = "",
                AccessTokenSecret = "",
                Name = "daenet",
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
