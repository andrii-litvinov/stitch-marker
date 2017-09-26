using System.IO;
using FluentAssertions;
using Ploeh.AutoFixture.Xunit2;
using SM.Core.Model;
using SM.Service.Classes;
using SM.Xsd;
using Xunit;
using Stitch = SM.Core.Model.Stitch;

namespace SM.Service.Tests
{
    public class ThumbnailDrawerShould
    {
        [Theory]
        [AutoData]
        public void DrawExpectedImage(XsdPatternReader patternReader, ThumbnailDrawer sut)
        {
            //Arrange
            var state = patternReader.Read(File.ReadAllBytes("Resources/M198_Seaside beauty.xsd"));

            //Act
            var image = sut.Draw(state);
            
            //Assert
            image.Should().Equal(File.ReadAllBytes("Resources/M198_Seaside beauty.png"));
        }

        [Theory]
        [AutoData]
        public void DrawSimpleImage(ThumbnailDrawer sut)
        {
            //Arrange
            var state = new PatternState
            {
                Width = 5,
                Height = 5,
                Stitches =
                {
                    new Stitch {ConfigurationIndex = 0, Point = new Point {X = 1, Y = 1}, Type = StitchType.Full},
                    new Stitch {ConfigurationIndex = 1, Point = new Point {X = 1, Y = 2}, Type = StitchType.Full},
                    new Stitch {ConfigurationIndex = 2, Point = new Point {X = 2, Y = 1}, Type = StitchType.Full}
                },
                Configurations =
                {
                    new StitchConfiguration {HexColor = "#ff0000"},
                    new StitchConfiguration {HexColor = "#00ff00"},
                    new StitchConfiguration {HexColor = "#0000ff"}
                }
            };

            //Act
            var image = sut.Draw(state);
            
            //Assert
            image.Should().Equal(File.ReadAllBytes("Resources/SimpleImage.png"));
        }
    }
}
