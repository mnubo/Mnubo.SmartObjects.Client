using System;
using Mnubo.SmartObjects.Client.Config;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using static Mnubo.SmartObjects.Client.Config.ClientConfig;
using System.Net;
using Newtonsoft.Json;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class HttpClient : IDisposable
    {
        internal static readonly Dictionary<Environments, string> addressMapping = new Dictionary<Environments, string>()
        {
            { Environments.Sandbox, "rest.sandbox.mnubo.com" },
            { Environments.Production, "rest.api.mnubo.com" }
        };
        internal const string DefaultClientSchema = "https";
        internal const string DefaultScope = "ALL";
        internal const int DefaultHostPort = 443;

        private const string DefaultBasePath = "/api/v3/";

        private readonly CredentialHandler credentialHandler;
        private readonly ClientConfig config;
        private readonly System.Net.Http.HttpClient client;

        internal HttpClient(ClientConfig config)
        {
            client = new System.Net.Http.HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(config.ClientTimeout);
            client.MaxResponseContentBufferSize = config.MaxResponseContentBufferSize;

            credentialHandler = new CredentialHandler(config, client);

            this.config = config;
        }

        internal async Task<string> sendAsyncRequestWithResult(HttpMethod method, string path)
        {
            return await sendAsyncRequestWithResult(method, path, null);
        }

        internal async Task sendAsyncRequest(HttpMethod method, string path, string body)
        {
            await sendAsyncRequestWithResult(method, path, body);
        }

        internal async Task sendAsyncRequest(HttpMethod method, string path)
        {
            await sendAsyncRequest(method, path, null);
        }

        internal async Task<string> sendAsyncRequestWithResult(HttpMethod method, string path, string body)
        {
            HttpRequestMessage request = null;
            HttpResponseMessage response = null;
            try
            {
                UriBuilder uriBuilder = new UriBuilder(DefaultClientSchema, HttpClient.addressMapping[config.Environment], DefaultHostPort, DefaultBasePath + path);

                request = new HttpRequestMessage(method, uriBuilder.Uri);
                request.Headers.Add("Authorization", credentialHandler.GetAuthenticationToken());

                if (!string.IsNullOrEmpty(body))
                {
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }

                response = await client.SendAsync(request);

                string message = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK ||
                    response.StatusCode == HttpStatusCode.Created)
                {
                    return message;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new ArgumentException(GetMessageFromReponse(message));
                }

                throw new InvalidOperationException(
                    string.Format("status code: {0}, message {1}", response.StatusCode, GetMessageFromReponse(message)));
            }
            finally
            {
                if (request != null)
                {
                    request.Dispose();
                }
                if (response != null)
                {
                    response.Dispose();
                }
            }
        }

        private string GetMessageFromReponse(string responseMessage)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(responseMessage)["message"];
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}