using System.IO;
using FluentAssertions;
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
            var pattern = sut.Read(File.ReadAllBytes("Resources/M198_Seaside beauty.xsd"));

            // Assert
            pattern.Stitches.Should().HaveCount(18775);
        }
    }
}
