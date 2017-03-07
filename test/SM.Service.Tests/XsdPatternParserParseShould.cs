using SM.Service.Xsd;
using System;
using Xunit;
using FluentAssertions;

namespace SM.Service.Tests
{
    public class XsdPatternParserShould
    {
        [Fact]
        public void ParseXsdPatterFile()
        {
            // Arrange
            var sut = new XsdPatternParser();

            // Act
            var pattern = sut.Parse(new byte[0]);

            // Assert
            pattern.Should().NotBeNull();
        }
    }
}