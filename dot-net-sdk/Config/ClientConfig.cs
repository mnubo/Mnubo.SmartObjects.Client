using System;

namespace Mnubo.SmartObjects.Client.Config
{
    public sealed class ClientConfig
    {
        /// <summary>
        /// Available enviroments
        /// </summary>
        public enum Environments
        {
            Sandbox,
            Production
        }

        /// <summary>
        /// Gets the Hostname server
        /// </summary>
        public Environments Environment { get; }

        /// <summary>
        /// get unique identity key provided by mnubo.
        /// </summary>
        public String ConsumerKey { get; }

        /// <summary>
        /// get secret key provided by mnubo.
        /// </summary>
        public String ConsumerSecret { get; }

        /// <summary>
        /// Timeout in miliseconds to use for requests made by this client instance.
        /// </summary>
        public int ClientTimeout { get; }

        /// <summary>
        /// Max number of bytes of buffer when reading the response content.
        /// </summary>
        public long MaxResponseContentBufferSize { get; }

        private ClientConfig() { }

        private ClientConfig(
            Environments environment,
            String consumerKey,
            String consumerSecret,
            int clientTimeout,
            long maxResponseContentBufferSize)
        {
            if (String.IsNullOrEmpty(consumerKey))
            {
                throw new ArgumentException("securityConsumerKey property cannot be blank.");
            }

            if (String.IsNullOrEmpty(consumerSecret))
            {
                throw new ArgumentException("securityConsumerSecret property cannot be blank.");
            }

            if (clientTimeout < 0)
            {
                throw new ArgumentException("clientTimeout must be a positive number.");
            }

            if (maxResponseContentBufferSize < 0)
            {
                throw new ArgumentException("maxResponseContentBufferSize must be a positive number.");
            }

            this.Environment = environment;
            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecret;
            this.ClientTimeout = clientTimeout;
            this.MaxResponseContentBufferSize = maxResponseContentBufferSize;
        }

        /// <summary>
        /// Config builder class, allow build a new immutable mnubo's config class.
        /// </summary>
        public sealed class Builder
        {
            public const int DefaultTimeout = 30000;
            public const long DefaultMaxResponseCcontentBufferSize = 1000000;

            /// <summary>
            /// build a new immutable mnubo's config instance from the builder.
            /// </summary>
            /// <param name="builder">Mnubo's cient Config builder</param>
            public static implicit operator ClientConfig(Builder builder)
            {
                return builder.Build();
            }

            /// <summary>
            /// The Hostname server
            /// </summary>
            public Environments Environment { get; set; }

            /// <summary>
            /// The unique identity key provided by mnubo.
            /// </summary>
            public String ConsumerKey { get; set; }

            /// <summary>
            /// The secret key provided by mnubo.
            /// </summary>
            public String ConsumerSecret { get; set; }

            /// <summary>
            /// Timeout in miliseconds to use for requests made by this client instance.
            /// </summary>
            public int ClientTimeout { get; set; }

            /// <summary>
            /// Max number of bytes of buffer when reading the response content.
            /// </summary>
            public long MaxResponseContentBufferSize { get; set; }

            public Builder()
            {
                this.ClientTimeout = DefaultTimeout;
                this.MaxResponseContentBufferSize = DefaultMaxResponseCcontentBufferSize;
            }

            public ClientConfig Build()
            {
                return new ClientConfig(
                    Environment,
                    ConsumerKey,
                    ConsumerSecret,
                    ClientTimeout,
                    MaxResponseContentBufferSize);
            }
        }
    }
}
