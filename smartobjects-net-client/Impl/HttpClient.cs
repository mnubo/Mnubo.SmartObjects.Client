using Polly;
using Polly.Retry;
using static Mnubo.SmartObjects.Client.Config.ClientConfig;
using Mnubo.SmartObjects.Client.Config;
using Newtonsoft.Json;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Net.Http.Headers;

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
        private readonly bool compressionEnabled = ClientConfig.Builder.DefaultCompressionEnabled;
        private readonly CredentialHandler credentialHandler;
        private readonly Environments environment;
        private readonly System.Net.Http.HttpClientHandler handler;
        private readonly System.Net.Http.HttpClient client;

        private readonly RetryPolicy<HttpResponseMessage> policy;

        internal HttpClient(ClientConfig config) : this(config, DefaultClientSchema, HttpClient.addressMapping[config.Environment], DefaultHostPort, DefaultBasePath) { }

        internal HttpClient(ClientConfig config, string clientSchema, string hostname, int hostPort, string basePath)
        {
            this.handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };

            this.client = new System.Net.Http.HttpClient(handler);
            this.client.Timeout = TimeSpan.FromMilliseconds(config.ClientTimeout);
            this.client.MaxResponseContentBufferSize = config.MaxResponseContentBufferSize;
            this.credentialHandler = new CredentialHandler(config, client, clientSchema, hostname, hostPort);

            this.environment = config.Environment;
            this.compressionEnabled = config.CompressionEnabled;
            this.clientSchema = clientSchema;
            this.hostname = hostname;
            this.hostPort = hostPort;
            this.basePath = basePath;

            this.policy = config.ExponentialBackoffConfig.Policy();
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

        private async Task<HttpResponseMessage> sendRequest(HttpMethod method, string path, string body) {
            HttpRequestMessage request = null;
            try
            {
                var pathAndQuery = path.Split(new char[] { '?' });
                UriBuilder uriBuilder = new UriBuilder(clientSchema, hostname, hostPort, basePath + pathAndQuery[0]);
                if(pathAndQuery.Length > 1)
                {
                    uriBuilder.Query = pathAndQuery[1];
                }

                request = new HttpRequestMessage(method, uriBuilder.Uri);
                request.Headers.Add("Authorization", credentialHandler.GetAuthenticationToken());

                if (!string.IsNullOrEmpty(body))
                {
                    if(compressionEnabled)
                    {
                        var data = Encoding.UTF8.GetBytes(body);
                        var stream = new MemoryStream();
                        using (var gz = new GZipStream(stream, CompressionMode.Compress)) 
                        {
                            await gz.WriteAsync(data, 0, data.Length);
                        }

                        var compressed = stream.ToArray();
                        stream.Dispose();
                        
                        request.Content = new ByteArrayContent(compressed);
                        request.Content.Headers.ContentEncoding.Add("gzip");
                    }
                    else
                    {
                        request.Content = new StringContent(body, Encoding.UTF8);
                    }

                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }
                return await client.SendAsync(request);
            }
            finally
            {
                if (request != null) {
                    request.Dispose();
                }
            }
        }

        internal async Task<string> sendAsyncRequestWithResult(HttpMethod method, string path, string body)
        {
            HttpResponseMessage response = null;
            try
            {
                if (policy != null) {
                    response = await policy.ExecuteAsync(() => sendRequest(method, path, body));
                } else {
                    response = await sendRequest(method, path, body);
                }

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