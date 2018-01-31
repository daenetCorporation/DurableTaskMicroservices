using Daenet.DurableTaskMicroservices.Common.BaseClasses;
using DurableTask.Core;
using LinqToTwitter;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.Microservice.Common.Test
{
    public class GetTweetsTask : TaskBase<GetTweetsTaskInput, GetTweetTaskOutput>
    {
        protected override GetTweetTaskOutput RunTask(TaskContext context, GetTweetsTaskInput input, ILogger logger)
        {
            logger.LogInformation($"{nameof(GetTweetsTask)} entered.");

            //var credential = getCredential(input.Key, input.Secret);
            //var aToken = getToken(credential: credential, logger: logger);
            //var result = getTweet(input.Name, input.Count, aToken, logger);

            var result = getMentions(input, logger).Result;

            logger.LogInformation($"{nameof(GetTweetsTask)} exit.");

            return result;

        }

        /// <summary>
        /// Get mentions in tweet
        /// </summary>
        /// <param name="input"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task<GetTweetTaskOutput> getMentions(GetTweetsTaskInput input, ILogger logger)
        {
            logger.LogInformation($"Getting mention. Method {nameof(getMentions)}");

            GetTweetTaskOutput output = new GetTweetTaskOutput()
            {
                IsTweetAvailable = false
            };

            try
            {
                var auth = new SingleUserAuthorizer()
                {
                    CredentialStore = new SingleUserInMemoryCredentialStore()
                    {
                        ConsumerKey = input.ConsumerKey,
                        ConsumerSecret = input.ConsumerSecret,
                        AccessToken = input.AccessToken,
                        AccessTokenSecret = input.AccessTokenSecret
                    },
                };

                auth.AuthorizeAsync().Wait();
                var twitterCtx = new TwitterContext(auth);

                var tweets =
                     await
                     (from tweet in twitterCtx.Status
                      where tweet.Type == StatusType.Mentions &&
                      tweet.Count == input.Count
                      select tweet)
                     .ToListAsync();

                foreach (var tweet in tweets)
                {
                    output.LatestTweetId = tweet.StatusID.ToString();

                    logger.LogInformation($"Mention Id: {tweet.StatusID}");

                    output.IsTweetAvailable = true;
                }

                Task.Delay(15000).Wait();

                logger.LogInformation($"Exit method {nameof(getMentions)}");
            }
            catch (Exception ex)
            {
                logger.LogWarning($"{ex.Message}");
            }

            return output;
        }


        /// <summary>
        /// Get Tweets 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="count"></param>
        /// <param name="oauth"></param>
        private GetTweetTaskOutput getTweet(string name, int count, string oauth, ILogger logger)
        {
            logger.LogInformation($"Getting latest Tweet in {nameof(getTweet)}");

            var output = new GetTweetTaskOutput()
            {
                IsTweetAvailable = false
            };

            var client = getHttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", oauth);

            var res = client.GetAsync($"1.1/statuses/user_timeline.json?screen_name={name}&count={count}").Result;

            if (res.IsSuccessStatusCode)
            {
                var tweets = JsonConvert.DeserializeObject<JArray>(res.Content.ReadAsStringAsync().Result);

                foreach (var tweet in tweets)
                {
                    logger.LogInformation($"Time: {tweet["created_at"]}");
                    logger.LogInformation($"Tweet: {tweet["text"]}");
                    logger.LogInformation($"Tweet Id: {tweet["id"]}");

                    output.IsTweetAvailable = true;
                    output.LatestTweetId = tweet["id"].ToString();
                }
            }
            else
                logger.LogWarning(res.Content.ReadAsStringAsync().Result);

            return output;
        }


        /// <summary>
        /// Get access token
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        private string getToken(string credential, ILogger logger)
        {
            logger.LogInformation($"Getting access token. Method {nameof(getToken)}.");

            string aToken = string.Empty;
            var client = getHttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credential);

            var body = new StringContent("grant_type=client_credentials");
            body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var res = client.PostAsync("oauth2/token", body).Result;

            if (res.IsSuccessStatusCode)
            {
                logger.LogInformation($"Got access token successfully.");

                var jobject = JsonConvert.DeserializeObject<JObject>(res.Content.ReadAsStringAsync().Result);
                aToken = jobject["access_token"].Value<string>();
            }
            else
               logger.LogWarning(res.Content.ReadAsStringAsync().Result);

            return aToken;
        }


        /// <summary>
        /// Convert base 64 bit string with Twitter Consumer Key and Consumer Secret
        /// </summary>
        /// <param name="key">Twitter Consumer Key</param>
        /// <param name="secret">Twitter Consumer Secret</param>
        /// <returns></returns>
        private string getCredential(string key, string secret)
        {
            var strCredential = $"{key}:{secret}";
            var bArray = Encoding.UTF8.GetBytes(strCredential);
            var base64Str = Convert.ToBase64String(bArray);
            return base64Str;
        }


        /// <summary>
        /// Http Client with base uri, https://api.twitter.com/
        /// </summary>
        /// <returns></returns>
        private HttpClient getHttpClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.twitter.com/");

            return client;
        }
    }
}
