using System;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace SM.Xsd.Tests.Unit
{
    public class XsdPatternParserShould
    {
        [Fact]
        public void ParseXsdPatterFile()
        {
            // Arrange
            var sut = new XsdPatternReader();

            // Act
            var pattern = sut.Read(File.OpenRead("Resources/M198_Seaside beauty.xsd"));

            var json = JsonConvert.SerializeObject(pattern, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
            File.WriteAllText("pattern.json", json);
            Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), "pattern.json"));

            // Assert
            pattern.Stitches.Should().HaveCount(18775);
        }
    }
}