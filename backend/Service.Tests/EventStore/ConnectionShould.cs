using System;
using EventStore.ClientAPI;
using FluentAssertions;
using Xunit;

namespace Service.Tests.EventStore
{
    public class ConnectionShould : IDisposable
    {
        public ConnectionShould()
        {
            connection = new ReadWriteEventStoreConnection("ConnectTo=tcp://admin:changeit@localhost:1113");
        }

        public void Dispose()
        {
            connection?.Dispose();
        }

        private readonly IReadWriteEventStoreConnection connection;

        [Fact]
        public async void ThrowWhenNoServerAvailable()
        {
            var stream = $"test-{Guid.NewGuid()}";
            var eventData = new EventData(Guid.NewGuid(), "Event1", false, Array.Empty<byte>(), Array.Empty<byte>());
            var writeResult = await connection.AppendToStream(stream, ExpectedVersion.NoStream, eventData);

            writeResult.NextExpectedVersion.Should().Be(0);
        }
    }
}
