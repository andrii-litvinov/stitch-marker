using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace SM.Service.Tests
{
    public class ResourceJsonConverterShould
    {
        public ResourceJsonConverterShould(ITestOutputHelper output)
        {
            this.output = output;
        }

        private readonly ITestOutputHelper output;

        [Fact]
        public void M()
        {
            var json = JsonConvert.SerializeObject(
                new Resource {Links = new[] {"Sr"}, Value = new {Va1One = 1, Val2 = 5}},
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});

            output.WriteLine(json);
        }
    }

    [JsonConverter(typeof(ResourceJsonConverter))]
    public class Resource
    {
        public object Value { get; set; }
        public string[] Links { get; set; }
    }
    
    public class ResourceJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var resource = (Resource)value;
            NamingStrategy namingStrategy = null;
            
            if (serializer.ContractResolver is DefaultContractResolver resolver) namingStrategy = resolver.NamingStrategy;
            if(namingStrategy == null) namingStrategy = new DefaultNamingStrategy();

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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Resource);
        }
    }
}
