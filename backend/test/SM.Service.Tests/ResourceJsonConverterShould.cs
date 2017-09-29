using System.Collections.Generic;
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
        public void ConvertToJsonShould()
        {
            //Arrange
            var expectedJson =
                "{\"patternId\":1,\"patternName\":\"M198_Seaside beauty\",\"height\":300,\"width\":300," +
                "\"links\":[{\"rel\":\"self\",\"href\":\"/api/patterns/patternId\"}" +
                ",{\"rel\":\"thumbnail\",\"href\":\"/api/patterns/patternId/thumbnail\"}]}";

            //Act
            var json = JsonConvert.SerializeObject(
                new Resource
                {
                    Links = new List<Links>
                    {
                        new Links {Rel = "self", Href = "/api/patterns/patternId"},
                        new Links {Rel = "thumbnail", Href = "/api/patterns/patternId/thumbnail"}
                    },
                    Value = new {PatternId = 1, PatternName = "M198_Seaside beauty", Height = 300, Width = 300}
                },
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});

            //Assert
            json.ShouldBeEquivalentTo(expectedJson);
        }
    }
}
