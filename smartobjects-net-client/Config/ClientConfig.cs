using System;

namespace Mnubo.SmartObjects.Client.Config
{
    /// <summary>
    /// Client configuration class
    /// </summary>
    public sealed class ClientConfig
    {
        /// <summary>
        /// Available enviroments:
        ///
        /// https://smartobjects.mnubo.com/documentation/api_basics.html
        /// </summary>
        public enum Environments
        {
            /// <summary>
            /// Using this environment will target the sandbox
            /// </summary>
            Sandbox,

            /// <summary>
            /// Using this environment will target the production
            /// </summary>
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

        /// <summary>
        /// get if compressing data enabled
        /// </summary>
        public bool CompressionEnabled { get; }

        /// <summary>
        /// Exponential back off configuration
        /// </summary>
        public IExponentialBackoffConfig ExponentialBackoffConfig { get; }

        private ClientConfig() { }

        private ClientConfig(
            Environments environment,
            String consumerKey,
            String consumerSecret,
            int clientTimeout,
            long maxResponseContentBufferSize,
            bool compressionEnabled,
            IExponentialBackoffConfig exponentialBackoffConfig)
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

            Environment = environment;
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            ClientTimeout = clientTimeout;
            MaxResponseContentBufferSize = maxResponseContentBufferSize;
            CompressionEnabled = compressionEnabled;
            ExponentialBackoffConfig = exponentialBackoffConfig;
        }

        /// <summary>
        /// Config builder class, allow build a new immutable mnubo's config class.
        /// </summary>
        public sealed class Builder
        {   
            /// <summary>
            /// Default http request timeout in milliseconds.
            ///
            /// Can be overriden like:
            /// <code>
            ///  var builder = new ClientConfig.Builder();
            ///  builder.ClientTimeout = 50000;
            /// </code>
            /// </summary>
            public const int DefaultTimeout = 30000;

            /// <summary>
            /// Default maximum number of bytes to buffer
            ///
            /// Can be overriden like:
            /// <code>
            ///  var builder = new ClientConfig.Builder();
            ///  builder.MaxResponseContentBufferSize = 1500000;
            /// </code>
            /// </summary>
            public const long DefaultMaxResponseContentBufferSize = 1000000;
            
            /// <summary>
            /// Compression is enabled by default
            ///
            /// Can be overriden like:
            /// <code>
            ///  var builder = new ClientConfig.Builder();
            ///  builder.CompressionEnabled = false;
            /// </code>
            /// </summary>
            public const bool DefaultCompressionEnabled = true;

            /// <summary>
            /// Default HTTP Exponential back off configuration
            /// <code>
            ///  var builder = new ClientConfig.Builder();
            ///  builder.ExponentialBackoffConfig = new ExponentialBackoffConfig.On(5, 500);
            /// </code>
            /// </summary>
            public static readonly IExponentialBackoffConfig DefaultExponentialBackoffConfig = new ExponentialBackoffConfig.Off();

            /// <summary>
            /// Implicitly build a new immutable mnubo's config instance from the builder
            /// </summary>
            /// <param name="builder">mnubo's client Config builder</param>
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

            /// <summary>
            /// Enable compressing data (gzip format) for network requests (default: true)
            /// </summary>
            public bool CompressionEnabled { get; set; }

            /// <summary>
            /// HTTP exponential back off configuration
            /// </summary>
            public IExponentialBackoffConfig ExponentialBackoffConfig { get; set; }


            /// <summary>
            /// Build a Builder to help you generate an immutable ClientConfig using the
            /// the default value for ClientTimeout, MaxResponseContentBufferSize and CompressionEnabled
            ///
            /// Usage:
            /// <code>
            ///  var builder = new ClientConfig.Builder();
            ///  builder.Environment = ClientConfig.Environments.Sandbox;
            ///  builder.ConsumerKey = "KEY";
            ///  builder.ConsumerSecret = "SECRET";
            ///  var config = builder.build();
            /// </code>
            /// </summary>
            public Builder()
            {
                ClientTimeout = DefaultTimeout;
                MaxResponseContentBufferSize = DefaultMaxResponseContentBufferSize;
                CompressionEnabled = DefaultCompressionEnabled;
                ExponentialBackoffConfig = DefaultExponentialBackoffConfig;
            }

            /// <summary>
            /// Build a new immutable mnubo's config instance from the builder
            /// </summary>
            public ClientConfig Build()
            {
                return new ClientConfig(
                    Environment,
                    ConsumerKey,
                    ConsumerSecret,
                    ClientTimeout,
                    MaxResponseContentBufferSize,
                    CompressionEnabled,
                    ExponentialBackoffConfig);
            }
        }
    }
}
