using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using FluentAssertions;
using Google.Protobuf;
using SM.Service.Infrastructure.EventStore;
using SM.Service.Messages;
using Xunit;

namespace SM.Service.Tests.EventStore
{
    public class EventStoreShould
    {
        public EventStoreShould()
        {
            eventStore = new Infrastructure.EventStore.EventStore(
                new ReadWriteEventStoreConnection("ConnectTo=tcp://admin:changeit@localhost:1113"));
        }

        private readonly Infrastructure.EventStore.EventStore eventStore;

        [Fact]
        public async Task GetEvents()
        {
            // Arrange
            var event1 = new PatternUploaded
            {
                Id = Guid.NewGuid().ToString(),
                FileName = "pattern1.xsd",
                Content = ByteString.CopyFrom(
                    Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet, consectetur adipiscing elit."))
            };

            var event2 = new PatternUploaded
            {
                Id = Guid.NewGuid().ToString(),
                FileName = "pattern2.xsd",
                Content = ByteString.CopyFrom(
                    Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet, consectetur adipiscing elit."))
            };

            var actorName = $"test-{Guid.NewGuid()}";
            long version = ExpectedVersion.NoStream;
            version = await eventStore.PersistEventAsync(actorName, version + 1, event1);
            version = await eventStore.PersistEventAsync(actorName, version + 1, event2);

            // Act
            var events = new List<PatternUploaded>();
            version = await eventStore.GetEventsAsync(actorName, StreamPosition.Start, version,
                e => events.Add((PatternUploaded) e));

            // Assert
            version.Should().Be(2);
            events.Should().HaveCount(2);
            events[0].FileName.Should().Be("pattern1.xsd");
            Encoding.UTF8.GetString(events[0].Content.ToByteArray()).Should()
                .Be("Lorem ipsum dolor sit amet, consectetur adipiscing elit.");
            events[1].FileName.Should().Be("pattern2.xsd");
            Encoding.UTF8.GetString(events[1].Content.ToByteArray()).Should()
                .Be("Lorem ipsum dolor sit amet, consectetur adipiscing elit.");
        }

        [Fact]
        public async Task PesistEvent()
        {
            // Arrange
            var @event = new PatternUploaded
            {
                Id = Guid.NewGuid().ToString(),
                FileName = "pattern.xsd",
                Content = ByteString.CopyFrom(
                    Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet, consectetur adipiscing elit."))
            };

            // Act
            var version =
                await eventStore.PersistEventAsync($"test-{Guid.NewGuid()}", ExpectedVersion.NoStream + 1, @event);

            // Assert
            version.Should().Be(0);
        }

        [Fact]
        public async Task CompressEvent()
        {
            // Arrange
            var @event = new PatternUploaded
            {
                Id = Guid.NewGuid().ToString(),
                FileName = "pattern.xsd",
                Content = ByteString.CopyFrom(Encoding.UTF8.GetBytes(new string('c', 512 * 1024)))
            };
            var actorName = $"test-{Guid.NewGuid()}";

            // Act
            PatternUploaded recoveredEvent = null;
            var version = await eventStore.PersistEventAsync(actorName, ExpectedVersion.NoStream + 1, @event);
            version = await eventStore.GetEventsAsync(actorName, StreamPosition.Start, version,
                e => recoveredEvent = (PatternUploaded) e);

            // Assert
            version.Should().Be(1);
            recoveredEvent.FileName.Should().Be("pattern.xsd");
            recoveredEvent.Content.Should().HaveCount(512 * 1024);
            Encoding.UTF8.GetString(recoveredEvent.Content.ToByteArray()).Should().MatchRegex("c{512}");
        }
    }
}
