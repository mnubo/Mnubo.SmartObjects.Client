using System;
using Mnubo.SmartObjects.Client.Config;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class CredentialHandler
    {
        internal const string TokenPath = "oauth/token";
        internal const string TokenConsumerSeparation = ":";
        internal const string TokenGrandType = "grant_type";
        internal const string TokenGrandTypeValue = "client_credentials";
        internal const string TokenScope = "scope";
        internal const string TokenAuthentication = "Authorization";
        internal const string TokenAuthenticationType = "Basic";
        internal const long FletchingTokenInMiliseconds = 1000;

        private ClientConfig config;
        private Token token;
        private HttpRequestMessage tokenRequest;
        private DateTime expireTime;
        private System.Net.Http.HttpClient client;

        internal CredentialHandler(ClientConfig config)
        {
            this.config = config;
            client = new System.Net.Http.HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(config.ClientTimeout);
            client.MaxResponseContentBufferSize = config.MaxResponseContentBufferSize;

            UriBuilder uriBuilder = new UriBuilder(
                HttpClient.DefaultClientSchema,
                config.addressMapping[config.Environment], 
                HttpClient.DefaultHostPort, 
                TokenPath);
            uriBuilder.Query = 
                TokenGrandType + "=" + 
                TokenGrandTypeValue + "&" + 
                TokenScope + "=" + 
                HttpClient.DefaultScope;

            tokenRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri);
            tokenRequest.Headers.Authorization = new AuthenticationHeaderValue(TokenAuthenticationType, GetAutherizationToken());

            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            (sender, certificate, chain, errors) => true;

            RequestToken();

            ThrowIfTokenNullOrInvalid(token, "Error invalid token...");
        }

        internal string GetAuthenticationToken()
        {
            if (DateTime.Compare(DateTime.Now.AddSeconds(FletchingTokenInMiliseconds), expireTime) >= 0)
            {
                RequestToken();
            }
            ThrowIfTokenNullOrInvalid(token, "Error invalid token...");
            return token.TokenType + " " + token.AccessToken;
        }

        private void SetExpireTime(Token token)
        {
            ThrowIfTokenNullOrInvalid(token, "Error validing token...");
            this.expireTime = DateTime.Now.AddSeconds(token.ExpiresIn);
            this.token = token;
        }

        private void RequestToken()
        {
            try
            {
                var tokenRequestToken = client.SendAsync(tokenRequest);
                tokenRequestToken.Wait();

                if (tokenRequestToken.Result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new InvalidOperationException("Error fetching token...");
                }

                var tokenAsStringTask = tokenRequestToken.Result.Content.ReadAsStringAsync();
                tokenAsStringTask.Wait();

                SetExpireTime(JsonConvert.DeserializeObject<Token>(tokenAsStringTask.Result));
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Error fetching token...");
            }
        }

        private string GetAutherizationToken()
        {
            return Convert.ToBase64String(
                System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(
                    config.ConsumerKey +
                    TokenConsumerSeparation +
                    config.ConsumerSecret));
        }

        private void ThrowIfTokenNullOrInvalid(Token token, string msg)
        {
            if(token == null || string.IsNullOrEmpty(token.AccessToken) || string.IsNullOrEmpty(token.TokenType))
            {
                throw new InvalidOperationException(msg);
            }
        }

        public class Token
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("jti")]
            public string Jti { get; set; }

            [JsonProperty("scope")]
            public string Scope { get; set; }
        }
    }
}