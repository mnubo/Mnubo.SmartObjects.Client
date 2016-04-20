using System;
using Mnubo.SmartObjects.Client.Config;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class HttpClient : IDisposable
    {
        internal const string DefaultClientSchema = "https";
        internal const string DefaultBasePath = "/api/v3/";
        internal const string DefaultScope = "ALL";
        internal const int DefaultHostPort = 443;

        private CredentialHandler credentialHandler;
        private ClientConfig config;
        private System.Net.Http.HttpClient client;

        internal HttpClient(ClientConfig config)
        {
            credentialHandler = new CredentialHandler(config);

            client = new System.Net.Http.HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(config.ClientTimeout);
            client.MaxResponseContentBufferSize = config.MaxResponseContentBufferSize;

            this.config = config;
        }

        internal Task<string> sendAsyncRequestWithBody(HttpMethod method, string path, string body)
        {
            return sendAsyncRequest(method, path, new Dictionary<string, string>(), body);
        }

        internal Task sendAsyncRequest(HttpMethod method, string path, string body)
        {
            return sendAsyncRequest(method, path, new Dictionary<string, string>(), body);
        }

        internal Task sendAsyncRequest(HttpMethod method, string path)
        {
            return sendAsyncRequest(method, path, new Dictionary<string, string>(), null);
        }

        internal Task<string> sendAsyncRequest(HttpMethod method, string path, Dictionary<string, string> pathQueries, string body)
        {
            UriBuilder uriBuilder = new UriBuilder(DefaultClientSchema, config.addressMapping[config.Environment], DefaultHostPort, DefaultBasePath + path);
            List<string> queries = new List<string>();
            foreach (KeyValuePair<string, string> query in pathQueries)
            {
                queries.Add(query.Key + "=" + query.Value);
            }
            uriBuilder.Query = string.Join("&", queries.ToArray());

            HttpRequestMessage request = new HttpRequestMessage(method, uriBuilder.Uri);
            request.Headers.Add("Authorization", credentialHandler.GetAuthenticationToken());

            if (!string.IsNullOrEmpty(body))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            Task<string> taskResult = Task<string>.Factory.StartNew(() =>
            {
                try
                {
                    var taskRequest = client.SendAsync(request);
                    taskRequest.Wait();

                    var messageAsstringTask = taskRequest.Result.Content.ReadAsStringAsync();
                    messageAsstringTask.Wait();

                    if (taskRequest.Result.StatusCode != System.Net.HttpStatusCode.OK &&
                        taskRequest.Result.StatusCode != System.Net.HttpStatusCode.Created)
                    {
                        throw new InvalidOperationException(
                            string.Format("status code: {0}, message {1}", taskRequest.Result.StatusCode, messageAsstringTask.Result));
                    }
                    return messageAsstringTask.Result;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            });

            string x = "l";

            return taskResult;
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}