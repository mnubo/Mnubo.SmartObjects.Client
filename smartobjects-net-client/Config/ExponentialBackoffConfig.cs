using System;
using Polly;
using Polly.Retry;
using System.Net.Http;
using System.Net;

namespace Mnubo.SmartObjects.Client.Config
{
    /// <summary>
    /// Exponential back off configuration defines a policy.
    /// </summary>
    public interface IExponentialBackoffConfig {
        /// <summary>
        /// The policy to be used.
        /// Null if no retries are to be attempted.
        /// </summary>
        RetryPolicy<HttpResponseMessage> Policy();
    }

    /// <summary>
    /// See <see cref="IExponentialBackoffConfig" />
    /// </summary>
    public class ExponentialBackoffConfig {

        /// <summary>
        /// An implementation where no retries are attempted
        /// </summary>
        public class Off : IExponentialBackoffConfig {

            /// <summary>
            /// See <see cref="IExponentialBackoffConfig.Policy" />
            /// </summary>
            public RetryPolicy<HttpResponseMessage> Policy() {
                return null;
            }
        }


        /// <summary>
        /// An implementation where the number of retries is given or fallback to `DefaultNumberOfAttempts`
        /// and the delay between retries starts at `DefaultInitialBackoffIntervalInMillis` milliseconds
        /// and grow exponentially.
        /// </summary>
        public class On : IExponentialBackoffConfig {
            internal const int DefaultNumberOfAttempts = 5;
            internal const int DefaultInitialBackoffIntervalInMillis = 500;
            internal static Action<DelegateResult<System.Net.Http.HttpResponseMessage>, TimeSpan> DefaultOnRetry =
                (res, calculatedWaitDuration) => {
                    Console.WriteLine("Retrying in " + calculatedWaitDuration.TotalMilliseconds + "ms.");
                };
            internal static Func<System.Net.Http.HttpResponseMessage, Boolean> DefaultRetryPredicate =
                r => r.StatusCode == HttpStatusCode.ServiceUnavailable;
            private Random r = new Random();

            private RetryPolicy<HttpResponseMessage> _policy;

            /// <summary>
            /// See <see cref="IExponentialBackoffConfig.Policy" />
            /// </summary>
            public RetryPolicy<HttpResponseMessage> Policy() {
                return this._policy;
            }

            /// <summary>
            /// Use the default configuration, looks like:
            /// <code>
            /// Retrying in 1000ms.
            /// Retrying in 2000ms.
            /// Retrying in 4000ms.
            /// Retrying in 8000ms.
            /// Retrying in 16000ms.
            /// </code>
            /// </summary>
            public On() : this(DefaultNumberOfAttempts, DefaultInitialBackoffIntervalInMillis, DefaultRetryPredicate, DefaultOnRetry) { }

            /// <summary>
            /// Override the numberOfAttempts and the initialBackoffDelayInMillis
            /// </summary>
            public On(int numberOfAttempts, int initialBackoffDelayInMillis) : this(numberOfAttempts, initialBackoffDelayInMillis, DefaultRetryPredicate, DefaultOnRetry) {}

            /// <summary>
            /// Override the numberOfAttempts, the initialBackoffDelayInMillis and the `Action` executed
            /// on every retry
            /// </summary>
            public On(
                int numberOfAttempts,
                int initialBackoffDelayInMillis,
                Action<DelegateResult<System.Net.Http.HttpResponseMessage>, TimeSpan> onRetry) : this(numberOfAttempts, initialBackoffDelayInMillis, DefaultRetryPredicate, onRetry) {}

            /// <summary>
            /// Override the numberOfAttempts, the initialBackoffDelayInMillis, the retry predicate and the `Action` executed
            /// on every retry
            /// </summary>
            public On(
                int numberOfAttempts,
                int initialBackoffDelayInMillis,
                Func<System.Net.Http.HttpResponseMessage, Boolean> retryCondition,
                Action<DelegateResult<System.Net.Http.HttpResponseMessage>, TimeSpan> onRetry)
            {
                this._policy = Polly.Policy.HandleResult<HttpResponseMessage>(retryCondition)
                    .WaitAndRetryAsync(
                        numberOfAttempts,
                        attempt => TimeSpan.FromMilliseconds((initialBackoffDelayInMillis + r.Next(0, 100)) * Math.Pow(2, attempt)),
                        (res, time) => onRetry(res, time)
                    );
            }
        }
    }

    
}