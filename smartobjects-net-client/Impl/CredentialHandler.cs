using System;
using Mnubo.SmartObjects.Client.Config;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal interface CredentialHandler {
        string GetAuthenticationToken();
    }
    internal class ClientIdAndSecretCredentialHandler : CredentialHandler
    {
        private const string TokenPath = "oauth/token";
        private const string TokenConsumerSeparation = ":";
        private const string TokenGrandType = "grant_type";
        private const string TokenGrandTypeValue = "client_credentials";
        private const string TokenScope = "scope";
        private const string TokenAuthentication = "Authorization";
        private const string TokenAuthenticationType = "Basic";
        private const long FletchingTokenInMiliseconds = 1000;

        private readonly System.Net.Http.HttpClient client;
        private readonly HttpRequestMessage tokenRequest;
        private readonly string autorizationBasicToken;
        private Token token;

        internal ClientIdAndSecretCredentialHandler(ClientConfig config, System.Net.Http.HttpClient client)
        {
            this.client = client;
            autorizationBasicToken = Convert.ToBase64String(
                System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(
                    config.ConsumerKey +
                    TokenConsumerSeparation +
                    config.ConsumerSecret));

            UriBuilder uriBuilder = new UriBuilder(
                HttpClient.DefaultClientSchema,
                config.Hostname,
                HttpClient.DefaultHostPort,
                TokenPath);
            uriBuilder.Query =
                TokenGrandType + "=" +
                TokenGrandTypeValue + "&" +
                TokenScope + "=" +
                HttpClient.DefaultScope;

            tokenRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri);
            tokenRequest.Headers.Authorization = new AuthenticationHeaderValue(TokenAuthenticationType, autorizationBasicToken);

            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            (sender, certificate, chain, errors) => true;
        }

        internal ClientIdAndSecretCredentialHandler(ClientConfig config, System.Net.Http.HttpClient client, String scheme, String host, int port)
        {
            this.client = client;
            autorizationBasicToken = Convert.ToBase64String(
                System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(
                    config.ConsumerKey +
                    TokenConsumerSeparation +
                    config.ConsumerSecret));

            UriBuilder uriBuilder = new UriBuilder(
                scheme,
                host,
                port,
                TokenPath);
            uriBuilder.Query =
                TokenGrandType + "=" +
                TokenGrandTypeValue + "&" +
                TokenScope + "=" +
                HttpClient.DefaultScope;

            tokenRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri);
            tokenRequest.Headers.Authorization = new AuthenticationHeaderValue(TokenAuthenticationType, autorizationBasicToken);

            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            (sender, certificate, chain, errors) => true;
        }

        string CredentialHandler.GetAuthenticationToken()
        {
            if (token == null || token.IsExpired())
            {
                RequestToken();
            }

            return token.AccessToken;
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

                token = new Token(JsonConvert.DeserializeObject<Dictionary<string,object>>(tokenAsStringTask.Result));
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Error fetching token...");
            }
        }

        public class Token
        {
            private const string AccessTokenProperty = "access_token";
            private const string TokenTypeProperty = "token_type";
            private const string ExpiresInProperty = "expires_in";
            private const string ScopeProperty = "scope";

            private readonly DateTime expireTime;

            internal string AccessToken { get; }

            internal Token(Dictionary<string, object> attributes)
            {
                if (!attributes.ContainsKey(AccessTokenProperty) ||
                   !attributes.ContainsKey(TokenTypeProperty) ||
                   !attributes.ContainsKey(ExpiresInProperty))
                {
                    throw new InvalidOperationException("Error fetching token...");
                }

                AccessToken =
                    string.Format(
                        "{0} {1}",
                        attributes[TokenTypeProperty],
                        attributes[AccessTokenProperty]);

                expireTime = DateTime.Now.AddSeconds(Convert.ToDouble(attributes[ExpiresInProperty]));
            }

            internal bool IsExpired()
            {
                bool status = true;
                if (DateTime.Compare(DateTime.Now.AddSeconds(FletchingTokenInMiliseconds), expireTime) < 0)
                {
                    status = false;
                }
                return status;
            }
        }
    }
    internal class StaticTokenCredentialHandler : CredentialHandler {
        private String token;
        internal StaticTokenCredentialHandler(String token) {
            this.token = token;
        }
        string CredentialHandler.GetAuthenticationToken()
        {
            return "Bearer " + token;
        }
    }
}