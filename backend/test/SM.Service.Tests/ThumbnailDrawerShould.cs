using SM.Core.Model;
using SM.Service.Classes;
using Xunit;


namespace SM.Service.Tests
{
    public class ThumbnailDrawerShould
    {
        [Fact]
        public void DrawThumbnail()
        {
            //Arrange
            var sut = new ThumbnailDrawer();
            var state = new PatternState
            {
                Width = 100,
                Height = 100,
                Stitches =
                {
                    new Stitch
                    {
                        ConfigurationIndex = 0,
                        Point = new Point
                        {
                            X = 1,
                            Y = 1
                        },
                        Type = StitchType.Full
                    }
                },
                Configurations =
                {
                    new StitchConfiguration
                    {
                        HexColor = "#ffffff"
                    }
                }
            };
            //Act
            sut.Draw(state);

            //Assert
        }
    }
}
