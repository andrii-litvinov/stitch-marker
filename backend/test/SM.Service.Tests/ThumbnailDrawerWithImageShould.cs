using System.IO;
using SM.Service.Classes;
using SM.Xsd;
using Xunit;

namespace SM.Service.Tests
{
    public class ThumbnailDrawerWithImageShould
    {
        [Fact]
        public void DrawThumbnailFromImage()
        {
            //Arrange
            var sut = new ThumbnailDrawer();
            var state = new XsdPatternReader().Read(File.ReadAllBytes("Resources/M198_Seaside beauty.xsd"));
            
            //Act
            sut.Draw(state);
            
            //Assert
        }
    }
}
