﻿using System;
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
            var expectedJson = @"{
  ""id"": ""48316e9c-fa8c-45b7-9b95-b747d53fcb86"",
  ""width"": 300,
  ""height"": 300,
  ""title"": ""M198_Seaside beauty"",
  ""author"": """",
  ""company"": """",
  ""copyright"": """",
  ""links"": [
    {
      ""rel"": ""self"",
      ""href"": ""/api/patterns/48316e9c-fa8c-45b7-9b95-b747d53fcb86""
    },
    {
      ""rel"": ""thumbnail"",
      ""href"": ""/api/patterns/48316e9c-fa8c-45b7-9b95-b747d53fcb86/thumbnail""
    }
  ]
}".Replace("\r\n", "\n");
            var settings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
            var resource =
                new Resource<PatternItem>(new PatternItem {Id = "48316e9c-fa8c-45b7-9b95-b747d53fcb86", Title = "M198_Seaside beauty", Height = 300, Width = 300})
                {
                    Links =
                    {
                        new Link {Rel = "self", Href = "/api/patterns/48316e9c-fa8c-45b7-9b95-b747d53fcb86"},
                        new Link {Rel = "thumbnail", Href = "/api/patterns/48316e9c-fa8c-45b7-9b95-b747d53fcb86/thumbnail"}
                    }
                };

            //Act
            var json = JsonConvert.SerializeObject(resource, Formatting.Indented, settings).Replace("\r\n", "\n");

            //Assert
            json.Should().Be(expectedJson);
        }
    }
}
