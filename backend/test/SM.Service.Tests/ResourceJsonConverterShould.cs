using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ploeh.AutoFixture.Xunit2;
using SM.Service.Classes;
using Xunit;
using Xunit.Abstractions;

namespace SM.Service.Tests
{
    public class ResourceJsonConverterShould
    {
        private readonly ITestOutputHelper output;

        public ResourceJsonConverterShould(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [AutoData]
        public void ConvertToJsonInExpectedFormat()
        {
            //Arrange
            const string expectedJson = @"{
  ""patternId"": 1,
  ""patternName"": ""M198_Seaside beauty"",
  ""height"": 300,
  ""width"": 300,
  ""links"": [
    {
      ""ref"": ""self"",
      ""href"": ""/api/patterns/patternId""
    },
    {
      ""ref"": ""thumbnail"",
      ""href"": ""/api/patterns/patternId/thumbnail""
    }
  ]
}";
            var settings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
            var resource =
                new Resource(new {PatternId = 1, PatternName = "M198_Seaside beauty", Height = 300, Width = 300})
                {
                    Links =
                    {
                        new Link {Rel = "self", Href = "/api/patterns/patternId"},
                        new Link {Rel = "thumbnail", Href = "/api/patterns/patternId/thumbnail"}
                    }
                };

            //Act
            var json = JsonConvert.SerializeObject(resource, Formatting.Indented, settings).Replace("\r\n", "\n");

            //Assert
            json.Should().Be(expectedJson);
        }
    }
}
