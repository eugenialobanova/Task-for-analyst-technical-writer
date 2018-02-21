using System;
using Kontur.Core.Binary.Serialization;
using Kontur.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kontur.Echelon
{
    public class EchelonTaskOptions : IBinarySerializable
    {
        private static readonly EchelonRetry[] DefaultRetry = {EchelonRetry.Exponential()};
        public static readonly EchelonTaskOptions Default = new EchelonTaskOptions();

        public EchelonTaskOptions(
            EchelonRetry[] retryStrategies,
            int maxAttempts = 5,
            int initialDelaySeconds = 0,
            int timeToLiveSeconds = -1,
            int priority = 0
        )
        {
            RetryStrategies = (retryStrategies == null || retryStrategies.Length == 0) ? DefaultRetry : retryStrategies;
            MaxAttempts = maxAttempts;
            InitialDelaySeconds = initialDelaySeconds;
            TimeToLiveSeconds = timeToLiveSeconds;
            Priority = priority;
        }

        public EchelonTaskOptions(
            EchelonRetryStrategy retryStrategy,
            int retryDelaySeconds,
            int maxAttempts = 5,
            int initialDelaySeconds = 0,
            int timeToLiveSeconds = -1,
            int priority = 0)
            : this(
                new[] {new EchelonRetry(retryStrategy, retryDelaySeconds, maxAttempts)},
                maxAttempts,
                initialDelaySeconds,
                timeToLiveSeconds,
                priority)
        {
        }

        public EchelonTaskOptions()
            : this(DefaultRetry, 5)
        {
        }

        [JsonConstructor]
        private EchelonTaskOptions(
            EchelonRetry[] retryStrategies,
            EchelonRetryStrategy? retryStrategy,
            int? retryDelaySeconds,
            int? maxAttempts,
            int? initialDelaySeconds,
            int? timeToLiveSeconds,
            int? priority)
            : this(
                retryStrategies ?? new[]
                {
                    new EchelonRetry(
                        retryStrategy ?? EchelonRetryStrategy.Exponential,
                        retryDelaySeconds ?? 600,
                        maxAttempts ?? 5)
                },
                maxAttempts ?? 5,
                initialDelaySeconds ?? 0,
                timeToLiveSeconds ?? -1,
                priority ?? 0
            )
        {
        }

        [JsonProperty(PropertyName = "initialDelaySeconds")]
        public int InitialDelaySeconds { get; private set; }

        [JsonProperty(PropertyName = "ttlSeconds")]
        public int TimeToLiveSeconds { get; private set; }

        [JsonProperty(PropertyName = "priority", DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Priority { get; private set; }

        [JsonProperty(PropertyName = "maxAttempts")]
        public int MaxAttempts { get; private set; }

        [JsonProperty(PropertyName = "retryDelaySeconds")]
        public int RetryDelaySeconds => RetryStrategies[0].BaseDelaySeconds;

        [JsonProperty(PropertyName = "retryStrategy")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EchelonRetryStrategy RetryStrategy => RetryStrategies[0].RetryStrategy;

        [JsonProperty(PropertyName = "retryStrategies")]
        public EchelonRetry[] RetryStrategies { get; private set; }

 
    }
}
