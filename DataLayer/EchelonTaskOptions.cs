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

        #region Serialization members

        public void SerializeBinary(IBinarySerializer serializer)
        {
            serializer
                .Write((byte) SerializationVersion.Retry)
                .WriteList(RetryStrategies, (retry, bs) => bs.Write(retry))
                .Write(MaxAttempts)
                .Write(InitialDelaySeconds)
                .Write(TimeToLiveSeconds)
                .Write(Priority);
        }

        public static EchelonTaskOptions DeserializeBinary(IBinaryDeserializer deserializer)
        {
            var version = (SerializationVersion) deserializer.ReadByte();

            switch (version)
            {
                case SerializationVersion.OldVersion0:
                case SerializationVersion.OldVersion1:
                case SerializationVersion.OldVersion2:
                    return DeserializeBinaryOldVersion(deserializer, (EchelonRetryStrategy) version);
                case SerializationVersion.Retry:
                    return DeserializeBinaryV3(deserializer);
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), $"Unsupported serialization version for EchelonTaskOptions: ({((int) version).ToString()})");
            }
        }

        private static EchelonTaskOptions DeserializeBinaryOldVersion(IBinaryDeserializer deserializer, EchelonRetryStrategy retryStrategy)
        {
            return new EchelonTaskOptions(
                retryStrategy,
                deserializer.ReadInt32(),
                deserializer.ReadInt32(),
                deserializer.ReadInt32(),
                deserializer.ReadInt32(),
                deserializer.ReadInt32()
            );
        }

        private static EchelonTaskOptions DeserializeBinaryV3(IBinaryDeserializer deserializer)
        {
            return new EchelonTaskOptions(
                deserializer.ReadArray(EchelonRetry.DeserializeBinary),
                deserializer.ReadInt32(),
                deserializer.ReadInt32(),
                deserializer.ReadInt32(),
                deserializer.ReadInt32()
            );
        }

        public int GetEstimatedSerializedSize()
            => SerializedSizes.Byte
               + SerializedSizes.Int32*4
               + SerializedSizes.List(EchelonRetry.SerializedSize, RetryStrategies.Length);

        private enum SerializationVersion : byte
        {
            OldVersion0 = 0,
            OldVersion1 = 1,
            OldVersion2 = 2,
            Retry = 3
        }

        #endregion

        #region Equality members

        protected bool Equals(EchelonTaskOptions other)
        {
            return InitialDelaySeconds == other.InitialDelaySeconds
                   && TimeToLiveSeconds == other.TimeToLiveSeconds
                   && Priority == other.Priority
                   && MaxAttempts == other.MaxAttempts
                   && RetryStrategies.ElementwiseEquals(other.RetryStrategies);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((EchelonTaskOptions) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = InitialDelaySeconds;
                hashCode = (hashCode*397) ^ TimeToLiveSeconds;
                hashCode = (hashCode*397) ^ Priority;
                hashCode = (hashCode*397) ^ MaxAttempts;
                hashCode = (hashCode*397) ^ RetryStrategies.ElementwiseHash();
                return hashCode;
            }
        }

        #endregion
    }
}