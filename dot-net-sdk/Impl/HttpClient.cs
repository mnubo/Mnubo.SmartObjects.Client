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

        private readonly string clientSchema;
        private readonly string hostname;
        private readonly int hostPort;
        private readonly string basePath;
        private readonly CredentialHandler credentialHandler;
        private readonly Environments enviroment;
        private readonly System.Net.Http.HttpClient client;

        internal HttpClient(ClientConfig config)
        {
            client = new System.Net.Http.HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(config.ClientTimeout);
            client.MaxResponseContentBufferSize = config.MaxResponseContentBufferSize;
            
            credentialHandler = new CredentialHandler(config, client);

            enviroment = config.Environment;
            clientSchema = DefaultClientSchema;
            hostname = HttpClient.addressMapping[enviroment];
            hostPort = DefaultHostPort;
            basePath = DefaultBasePath;
        }

        internal HttpClient(ClientConfig config, string clientSchema, string hostname, int hostPort, string basePath )
        {
            client = new System.Net.Http.HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(config.ClientTimeout);
            client.MaxResponseContentBufferSize = config.MaxResponseContentBufferSize;
            credentialHandler = new CredentialHandler(config, client, clientSchema, hostname, hostPort);

            enviroment = config.Environment;
            this.clientSchema = clientSchema;
            this.hostname = hostname;
            this.hostPort = hostPort;
            this.basePath = basePath;
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
                UriBuilder uriBuilder = new UriBuilder(clientSchema, hostname, hostPort, basePath + path);

                request = new HttpRequestMessage(method, uriBuilder.Uri);
                request.Headers.Add("Authorization", credentialHandler.GetAuthenticationToken());

                if (!string.IsNullOrEmpty(body))
                {
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }

                response = await client.SendAsync(request);

                string message = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK ||
                    response.StatusCode == HttpStatusCode.Created ||
                    (int)response.StatusCode == 207)
                {
                    return message;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new ArgumentException(message);
                }

                throw new InvalidOperationException(
                    string.Format("status code: {0}, message {1}", response.StatusCode, message));
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

        public void Dispose()
        {
            client.Dispose();
        }
    }
}