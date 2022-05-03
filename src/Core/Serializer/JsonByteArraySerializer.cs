using Core.Serializer.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Core.Serializer
{
    public class JsonByteArraySerializer : IJsonByteArraySerializer
    {
        private readonly JsonSerializerSettings _settings = new() { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public JsonByteArraySerializer()
        {
            _settings.Converters.Add(new StringEnumConverter());
        }

        public byte[] Serialize(object data)
        {
            var jsonString = JsonConvert.SerializeObject(data, _settings);
            return Encoding.UTF8.GetBytes(jsonString);
        }

        public TData Deserialize<TData>(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<TData>(Encoding.UTF8.GetString(bytes), _settings);
        }
    }
}
