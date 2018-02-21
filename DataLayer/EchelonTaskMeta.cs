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

	}
}
