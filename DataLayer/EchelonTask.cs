using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kontur.Core.Binary.Serialization;
using Kontur.Utilities;
using Newtonsoft.Json;

namespace Kontur.Echelon
{
    [JsonObject]
    public class EchelonTask : IBinarySerializable, IEnumerable<KeyValuePair<string, string>>
    {
       

        public EchelonTask(string type, IDictionary<string, string> content)
            : this(Guid.NewGuid(), type, content)
        {
        }

        public EchelonTask(Guid id, string type)
            : this(id, type, new Dictionary<string, string>())
        {
        }

        public EchelonTask(string type)
            : this(type, new Dictionary<string, string>())
        {
        }

        [JsonConstructor]
        internal EchelonTask(Guid id, string type, IDictionary<string, string> content)
        {
            Id = id;
            Type = type;
            Content = content;
        }

        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public Guid Id { get; private set; }

        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public string Type { get; private set; }

        [JsonProperty(PropertyName = "content", Required = Required.Always)]
        public IDictionary<string, string> Content { get; private set; }

        public string this[string key]
        {
            get { return Content[key]; }
        }

        public void Add(string key, string value)
        {
            Content.Add(key, value);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Content.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Format("id: {0}, type: {1}, content: {2}", Id, Type, SerializationHelper.SerializeToJsonLine(Content));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region Equality members

        protected bool Equals(EchelonTask other)
        {
            return Id.Equals(other.Id)
                   && string.Equals(Type, other.Type, StringComparison.Ordinal)
                   && Content.ElementwiseEquals(other.Content);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((EchelonTask) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode*397) ^ Type.GetHashCode();
                hashCode = (hashCode*397) ^ Content.ElementwiseHash();
                return hashCode;
            }
        }

        #endregion
    }
}
