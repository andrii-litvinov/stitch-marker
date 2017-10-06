using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Ploeh.AutoFixture.Xunit2;
using Proto;
using SM.Service.Messages;
using SM.Service.Patterns;
using SM.Service.Patterns.Xsd;
using Xunit;

namespace SM.Service.Tests
{
    public class ThumbnailDrawerShould
    {
        [Theory]
        [AutoData]
        public async void DrawExpectedImage(XsdPatternReader patternReader)
        {
            //Arrange
            var props = Actor.FromProducer(() => new DrawerSuperviser());
            var pid = Actor.Spawn(props);
            var pattern = patternReader.Read(File.ReadAllBytes("Resources/M198_Seaside beauty.xsd"));

            //Act
            var request = await pid.RequestAsync<Thumbnail>(pattern, 3.Seconds());

            //Assert
            request.Image.Should().Equal(File.ReadAllBytes("Resources/M198_Seaside beauty.png"));
        }

        [Theory]
        [AutoData]
        public async void DrawSimpleImage()
        {
            //Arrange
            var props = Actor.FromProducer(() => new DrawerSuperviser());
            var pid = Actor.Spawn(props);
            var pattern = new Messages.Pattern
            {
                Width = 5,
                Height = 5,
                Stitches =
                {
                    new Messages.Stitch {ConfigurationIndex = 0, Point = new Point {X = 1, Y = 1}, Type = StitchType.Full},
                    new Messages.Stitch {ConfigurationIndex = 1, Point = new Point {X = 1, Y = 2}, Type = StitchType.Full},
                    new Messages.Stitch {ConfigurationIndex = 2, Point = new Point {X = 2, Y = 1}, Type = StitchType.Full}
                },
                Configurations =
                {
                    new StitchConfiguration {HexColor = "#ff0000"},
                    new StitchConfiguration {HexColor = "#00ff00"},
                    new StitchConfiguration {HexColor = "#0000ff"}
                }
            };

            //Act
            var request = await pid.RequestAsync<Thumbnail>(pattern, 3.Seconds());

            //Assert
            request.Image.Should().Equal(File.ReadAllBytes("Resources/SimpleImage.png"));
        }

        private class DrawerSuperviser : IActor
        {
            private PID requestor;

            public Task ReceiveAsync(IContext context)
            {
                switch (context.Message)
                {
                    case Messages.Pattern pattern:
                        var thumbnailActorProps = Actor.FromProducer(() => new PatternImageActor());
                        var thumbnailActorPid = context.Spawn(thumbnailActorProps);
                        thumbnailActorPid.Tell(new CreateThumbnail {Pattern = pattern});
                        requestor = context.Sender;
                        break;
                    case Thumbnail thumbnail:
                        requestor?.Tell(thumbnail);
                        break;
                }
                return Actor.Done;
            }
        }
    }
}
