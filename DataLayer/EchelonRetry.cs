using Kontur.Core.Binary.Serialization
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kontur.Echelon
{
    public class EchelonRetry : IBinarySerializable
    {
        [JsonConstructor]
        public EchelonRetry(EchelonRetryStrategy retryStrategy, int baseDelaySeconds, int attemptsCount)
        {
            AttemptsCount = attemptsCount;
            RetryStrategy = retryStrategy;
            BaseDelaySeconds = baseDelaySeconds;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public EchelonRetryStrategy RetryStrategy { get; }

        public int AttemptsCount { get; }

        public int BaseDelaySeconds { get; }

        public static EchelonRetry Linear(int baseDelaySeconds = 600, int attemptsCount = 5)
            => new EchelonRetry(EchelonRetryStrategy.Linear, baseDelaySeconds, attemptsCount);

        public static EchelonRetry LinearBackoff(int baseDelaySeconds = 600, int attemptsCount = 5)
            => new EchelonRetry(EchelonRetryStrategy.LinearBackoff, baseDelaySeconds, attemptsCount);

        public static EchelonRetry Exponential(int baseDelaySeconds = 600, int attemptsCount = 5)
            => new EchelonRetry(EchelonRetryStrategy.Exponential, baseDelaySeconds, attemptsCount);

      
    }
}
