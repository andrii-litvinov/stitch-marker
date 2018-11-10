using System;
using System.IO;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Proto;
using Service.Patterns;
using Service.Patterns.Xsd;
using Xunit;

namespace Service.Tests
{
    public class ThumbnailDrawerShould
    {
        [Theory]
        [AutoData]
        public async void DrawExpectedImage(RootContext context, XsdPatternReader patternReader)
        {
            //Arrange
            var props = Props.FromProducer(() => new Superviser());
            var pid = context.Spawn(props);
            var pattern = patternReader.Read(File.ReadAllBytes("Resources/M198_Seaside beauty.xsd"));

            //Act
            var thumbnail = await context.RequestAsync<Thumbnail>(pid, pattern);

            //Assert
            thumbnail.Image.ToByteArray().Should().Equal(File.ReadAllBytes("Resources/M198_Seaside beauty.png"));
        }

        private class Superviser : IActor
        {
            private PID requester;

            public Task ReceiveAsync(IContext context)
            {
                switch (context.Message)
                {
                    case Pattern pattern:
                        var thumbnailActorProps = Props.FromProducer(() => new PatternImageActor());
                        var thumbnailActorPid = context.Spawn(thumbnailActorProps);
                        context.Send(thumbnailActorPid, new GetThumbnail
                        {
                            Id = Guid.NewGuid().ToString(),
                            Width = 300,
                            Height = 200,
                            Pattern = pattern
                        });
                        requester = context.Sender;
                        break;
                    case Thumbnail thumbnail:
                        context.Send(requester, thumbnail);
                        break;
                }
                return Actor.Done;
            }
        }
    }
}
