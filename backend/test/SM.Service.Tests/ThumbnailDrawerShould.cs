using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using AutoFixture.Xunit2;
using Proto;
using SM.Service.Patterns;
using SM.Service.Patterns.Xsd;
using Xunit;
using Pattern = SM.Service.Pattern;

namespace SM.Service.Tests
{
    public class ThumbnailDrawerShould
    {
        [Theory]
        [AutoData]
        public async void DrawExpectedImage(XsdPatternReader patternReader)
        {
            //Arrange
            var props = Actor.FromProducer(() => new Superviser());
            var pid = Actor.Spawn(props);
            var pattern = patternReader.Read(File.ReadAllBytes("Resources/M198_Seaside beauty.xsd"));

            //Act
            var thumbnail = await pid.RequestAsync<Thumbnail>(pattern);

            //Assert
            thumbnail.Image.ToByteArray().Should().Equal(File.ReadAllBytes("Resources/M198_Seaside beauty.png"));
        }

        private class Superviser : IActor
        {
            private PID requestor;

            public Task ReceiveAsync(IContext context)
            {
                switch (context.Message)
                {
                    case Pattern pattern:
                        var thumbnailActorProps = Actor.FromProducer(() => new PatternImageActor());
                        var thumbnailActorPid = context.Spawn(thumbnailActorProps);
                        thumbnailActorPid.Tell(new GetThumbnail
                        {
                            Id = Guid.NewGuid().ToString(),
                            Width = 300,
                            Height = 200,
                            Pattern = pattern
                        });
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
