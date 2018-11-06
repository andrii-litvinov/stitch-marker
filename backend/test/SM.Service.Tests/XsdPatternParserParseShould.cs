using System.IO;
using FluentAssertions;
using SM.Service.Patterns;
using SM.Service.Patterns.Xsd;
using Xunit;

namespace SM.Service.Tests
{
    public class XsdPatternParserShould
    {
        [Fact]
        public void ParseXsdPatternFile()
        {
            // Arrange
            var sut = new XsdPatternReader();

            // Act
            var pattern = sut.Read(File.ReadAllBytes("Resources/M198_Seaside beauty.xsd"));

            // Assert
            pattern.Stitches.Should().HaveCount(18775);
        }
    }
}
