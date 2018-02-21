using System;
using System.Net;
using Kontur.Core.Binary.Serialization;
using Newtonsoft.Json;

namespace Kontur.Echelon
{
	public class EchelonTaskMeta : IBinarySerializable
	{
		public EchelonTaskMeta(
            long creationTimeUtc, 
            long executionTimeUtc, 
            int attemptsUsed, 
            string producerIdentity, 
            IPAddress producerIp, 
            string producerTopology)
		{
		    ProducerTopology = producerTopology;
		    CreationTimeUtc = creationTimeUtc;
			ExecutionTimeUtc = executionTimeUtc;
			AttemptsUsed = attemptsUsed;
			ProducerIdentity = producerIdentity;
			ProducerIP = producerIp;
		}

		public void SerializeBinary(IBinarySerializer serializer)
		{
			serializer
                .Write(SerializationMarkerV2)
				.Write(CreationTimeUtc)
				.Write(ExecutionTimeUtc)
				.Write(AttemptsUsed)
				.Write(ProducerIdentity)
				.Write(ProducerIP)
                .Write(ProducerTopology);
		}

		public static EchelonTaskMeta DeserializeBinary(IBinaryDeserializer deserializer)
		{
		    long value = deserializer.ReadInt64();

		    return value == SerializationMarkerV2 
                ? DeserializeBinaryV2(deserializer) 
                : DeserializeBinaryV1(deserializer, value);
		}
	   
	    public int GetEstimatedSerializedSize()
		{
			return SerializedSizes.Int64 * 2 
				+ SerializedSizes.Int32
				+ ProducerIdentity.Length * 2
                + ProducerTopology.Length * 2
				+ SerializedSizes.IPAddress 
				+ SerializedSizes.Guid;
		}

		[JsonProperty(PropertyName = "creationTimeUtc", Required = Required.Always)]
		public long CreationTimeUtc { get; private set; }

		[JsonProperty(PropertyName = "executionTimeUtc", Required = Required.Always)]
		public long ExecutionTimeUtc { get; private set; }

		[JsonProperty(PropertyName = "attemptsUsed", Required = Required.Always)]
		public int AttemptsUsed { get; private set; }

		[JsonProperty(PropertyName = "producerIdentity", Required = Required.Always)]
		public string ProducerIdentity { get; private set; }

		[JsonProperty(PropertyName = "producerIP", Required = Required.Always)]
		[JsonConverter(typeof(IPAddressConverter))]
		public IPAddress ProducerIP { get; private set; }

        [JsonProperty(PropertyName = "producerTopology", Required = Required.Always)]
        public string ProducerTopology { get; private set; }

        private static EchelonTaskMeta DeserializeBinaryV2(IBinaryDeserializer deserializer)
        {
            return new EchelonTaskMeta(
                deserializer.ReadInt64(),
                deserializer.ReadInt64(),
                deserializer.ReadInt32(),
                deserializer.ReadString(),
                deserializer.ReadIPAddress(),
                deserializer.ReadString()
            );
        }

        private static EchelonTaskMeta DeserializeBinaryV1(IBinaryDeserializer deserializer, long creationTimeUtc)
        {
            return new EchelonTaskMeta(
                creationTimeUtc,
                deserializer.ReadInt64(),
                deserializer.ReadInt32(),
                deserializer.ReadString(),
                deserializer.ReadIPAddress(),
                String.Empty
            );
        }

		#region Equality members
		protected bool Equals(EchelonTaskMeta other)
		{
			return CreationTimeUtc == other.CreationTimeUtc
				&& ExecutionTimeUtc == other.ExecutionTimeUtc
				&& AttemptsUsed == other.AttemptsUsed
				&& string.Equals(ProducerIdentity, other.ProducerIdentity)
                && string.Equals(ProducerTopology, other.ProducerTopology)
				&& ProducerIP.Equals(other.ProducerIP);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((EchelonTaskMeta)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = CreationTimeUtc.GetHashCode();
				hashCode = (hashCode * 397) ^ ExecutionTimeUtc.GetHashCode();
				hashCode = (hashCode * 397) ^ AttemptsUsed;
                hashCode = (hashCode * 397) ^ ProducerTopology.GetHashCode();
				hashCode = (hashCode * 397) ^ ProducerIdentity.GetHashCode();
				hashCode = (hashCode * 397) ^ ProducerIP.GetHashCode();
				return hashCode;
			}
		} 
		#endregion

	    private const long SerializationMarkerV2 = -2L;
	}
}