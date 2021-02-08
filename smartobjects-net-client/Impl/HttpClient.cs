using Polly.Retry;
using Mnubo.SmartObjects.Client.Config;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Reflection;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal interface IHttpClient
    {
        Task<string> SendAsyncRequestWithResult(HttpMethod method, string path);
        
        Task SendAsyncRequest(HttpMethod method, string path, string body);
        
        Task SendAsyncRequest(HttpMethod method, string path);
        
        Task<HttpResponseMessage> SendRequest(HttpMethod method, string path, string body);
        
        Task<string> SendAsyncRequestWithResult(HttpMethod method, string path, string body);
    }

    internal class HttpClient : IDisposable, IHttpClient
    {
        internal const string DefaultClientSchema = "https";
        internal const string DefaultScope = "ALL";
        internal const int DefaultHostPort = 443;

        private const string DefaultBasePath = "";

        private readonly string clientSchema;
        private readonly string hostname;
        private readonly int hostPort;
        private readonly string basePath;
        private readonly bool compressionEnabled = ClientConfig.Builder.DefaultCompressionEnabled;
        private readonly CredentialHandler credentialHandler;
        private readonly HttpClientHandler handler;
        private readonly System.Net.Http.HttpClient client;
        private readonly AsyncRetryPolicy<HttpResponseMessage> policy;
        private readonly string version;

        private static string LoadVersion() {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var textStreamReader = new StreamReader(assembly.GetManifestResourceStream("Mnubo.SmartObjects.Client.Version.txt"));
                var content = textStreamReader.ReadToEnd();
                return content.Split('=')[1].Replace(System.Environment.NewLine, "").TrimEnd(Environment.NewLine.ToCharArray());
            }
            catch
            {
                return "unknown";
            }
        }
        
        internal HttpClient(ClientConfig config) : this(config, DefaultClientSchema, config.Hostname, DefaultHostPort, DefaultBasePath) { }

        internal HttpClient(ClientConfig config, string clientSchema, string hostname, int hostPort, string basePath)
        {
            this.version = ".NET/" + LoadVersion();
            this.handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };

            this.client = new System.Net.Http.HttpClient(handler);
            this.client.Timeout = TimeSpan.FromMilliseconds(config.ClientTimeout);
            this.client.MaxResponseContentBufferSize = config.MaxResponseContentBufferSize;
            if (string.IsNullOrEmpty(config.Token)) {
                this.credentialHandler = new ClientIdAndSecretCredentialHandler(config, client, clientSchema, hostname, hostPort);
            } else {
                this.credentialHandler = new StaticTokenCredentialHandler(config.Token);
            }

            this.compressionEnabled = config.CompressionEnabled;
            this.clientSchema = clientSchema;
            this.hostname = hostname;
            this.hostPort = hostPort;
            this.basePath = basePath;

            this.policy = config.ExponentialBackoffConfig.Policy();
        }

        public async Task<string> SendAsyncRequestWithResult(HttpMethod method, string path)
        {
            return await SendAsyncRequestWithResult(method, path, null);
        }

        public async Task SendAsyncRequest(HttpMethod method, string path, string body)
        {
            await SendAsyncRequestWithResult(method, path, body);
        }

        public async Task SendAsyncRequest(HttpMethod method, string path)
        {
            await SendAsyncRequest(method, path, null);
        }

        public async Task<HttpResponseMessage> SendRequest(HttpMethod method, string path, string body) {
            HttpRequestMessage request = null;
            try
            {
                var pathAndQuery = path.Split(new char[] { '?' });
                var uriBuilder = new UriBuilder(clientSchema, hostname, hostPort, basePath + pathAndQuery[0]);
                if(pathAndQuery.Length > 1)
                {
                    uriBuilder.Query = pathAndQuery[1];
                }

                request = new HttpRequestMessage(method, uriBuilder.Uri);
                request.Headers.Add("Authorization", credentialHandler.GetAuthenticationToken());
                request.Headers.Add("X-MNUBO-SDK", this.version);

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
                request?.Dispose();
            }
        }

        public async Task<string> SendAsyncRequestWithResult(HttpMethod method, string path, string body)
        {
            HttpResponseMessage response = null;
            try
            {
                if (policy != null) {
                    response = await policy.ExecuteAsync(() => SendRequest(method, path, body));
                } else {
                    response = await SendRequest(method, path, body);
                }

                var message = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK ||
                    response.StatusCode == HttpStatusCode.Created ||
                    response.StatusCode == HttpStatusCode.NoContent ||
                    (int)response.StatusCode == 207)
                {
                    return message;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new ArgumentException(message);
                }

                throw new InvalidOperationException(
                    $"status code: {response.StatusCode}, message {message}");
            }
            finally
            {
                response?.Dispose();
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}