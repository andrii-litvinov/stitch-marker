using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace SM.Service.Resources
{
    public class ResourceJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var resource = (Resource) value;
            NamingStrategy namingStrategy = null;

            if (serializer.ContractResolver is DefaultContractResolver resolver)
                namingStrategy = resolver.NamingStrategy;
            if (namingStrategy == null) namingStrategy = new DefaultNamingStrategy();

            writer.WriteStartObject();

            foreach (var jToken in JToken.FromObject(resource.Value))
            {
                var item = (JProperty) jToken;
                writer.WritePropertyName(namingStrategy.GetPropertyName(item.Name, false));
                item.Value.WriteTo(writer);
            }

            writer.WritePropertyName(namingStrategy.GetPropertyName(nameof(resource.Links), false));
            serializer.Serialize(writer, resource.Links);

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Resource);
        }
    }
}
