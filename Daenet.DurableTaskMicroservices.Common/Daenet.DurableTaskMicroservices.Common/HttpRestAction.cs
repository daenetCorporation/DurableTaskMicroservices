using DurableTask.Microservices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Daenet.DurableTaskMicroservices.Common
{
    /// <summary>
    /// Implements a helper class, which provides simplified methods for HTTP.
    /// </summary>
    public class HttpRestAction
    {
        /// <summary>
        /// Implements HttpPost helper method.
        /// </summary>
        /// <typeparam name="TOutput">The resulting type, which will be used for result deserialization.</typeparam>
        /// <param name="authToken">OAuth token used for authentication by service.</param>
        /// <param name="serviceUrl">The service operation URL.</param>
        /// <param name="objectInstance">The instance of the object which will be sent to the
        /// service operation as JSON serialized body.</param>
        /// <returns></returns>
        public static TOutput Post<TOutput>(string authToken, string serviceUrl, object objectInstance)
        {
            int maxRetries = 5;
            int delayInterval = 5000;
            Exception error = null;
            TOutput res;

            while (maxRetries > 0)
            {
                try
                {
                    res = execHttpPost<TOutput>(authToken, serviceUrl, objectInstance);
                    return res;
                }
                catch(Exception ex)
                {
                    error = ex;
                    maxRetries--;

                    if (maxRetries > 0)
                        Thread.Sleep(delayInterval);
                }                
            }

            throw error;
        }

        private static TOutput execHttpPost<TOutput>(string authToken, string serviceUrl, object objectInstance)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create(serviceUrl);

            // Set the Method property of the request to POST.
            request.Method = "POST";

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/json;charset=utf-8";

            request.Headers[HttpRequestHeader.Authorization] = authToken;

            request.PreAuthenticate = false;

            byte[] byteArray = null;

            string data = JsonConvert.SerializeObject(objectInstance);

            byteArray = Encoding.UTF8.GetBytes(data);

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            Stream dataStream = request.GetRequestStream();

            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);

            // Close the Stream object.
            dataStream.Close();

            // Get the response.
            WebResponse response = request.GetResponse();

            TOutput res;

            HttpWebResponse httpResponse = response as HttpWebResponse;

            if (httpResponse.StatusCode != HttpStatusCode.OK &&
                httpResponse.StatusCode != HttpStatusCode.NoContent)
                throw new ApplicationException(String.Format("{0} - {1}", httpResponse.StatusCode, httpResponse.StatusDescription));

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();

                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);

                // Read the content.
                string responseFromServer = reader.ReadToEnd();

                if (typeof(TOutput) == typeof(Null))
                    res = Activator.CreateInstance<TOutput>();
                else
                    res = JsonConvert.DeserializeObject<TOutput>(responseFromServer);
                // Clean up the streams.
                reader.Close();
                dataStream.Close();

            }
            else
            {
                if (typeof(TOutput) == typeof(Null))
                    res = Activator.CreateInstance<TOutput>();
                else
                    throw new InvalidOperationException("Unexpected result. REST operations which return void (no content) should specify 'Null' as return type of this task.");
            }

            response.Close();

            return res;
        }
    }
}
