using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using AutoFixture.Xunit2;
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
            string expectedJson = @"{
  ""patternId"": 1,
  ""patternName"": ""M198_Seaside beauty"",
  ""height"": 300,
  ""width"": 300,
  ""links"": [
    {
      ""rel"": ""self"",
      ""href"": ""/api/patterns/1""
    },
    {
      ""rel"": ""thumbnail"",
      ""href"": ""/api/patterns/1/thumbnail""
    }
  ]
}".Replace("\r\n", "\n");
            var settings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
            var resource =
                new Resource(new {PatternId = 1, PatternName = "M198_Seaside beauty", Height = 300, Width = 300})
                {
                    Links =
                    {
                        new Link {Rel = "self", Href = "/api/patterns/1"},
                        new Link {Rel = "thumbnail", Href = "/api/patterns/1/thumbnail"}
                    }
                };

            //Act
            var json = JsonConvert.SerializeObject(resource, Formatting.Indented, settings).Replace("\r\n", "\n");

            //Assert
            json.Should().Be(expectedJson);
        }
    }
}
