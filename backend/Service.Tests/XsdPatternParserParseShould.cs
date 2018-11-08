using System.IO;
using FluentAssertions;
using Service.Patterns.Xsd;
using Xunit;

namespace Service.Tests
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
