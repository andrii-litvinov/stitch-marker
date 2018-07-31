using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace SM.Service
{
    public class ReadWriteEventStoreConnection : IReadWriteEventStoreConnection
    {
        private readonly string connectionString;
        private Lazy<Task<IEventStoreConnection>> connection;

        public ReadWriteEventStoreConnection(string connectionString)
        {
            this.connectionString = connectionString;
            Reconnect();
        }

        public async Task<WriteResult> AppendToStream(string stream, long expectedVersion, params EventData[] events)
        {
            return await (await connection.Value).AppendToStreamAsync(stream, expectedVersion, events);
        }

        public void Dispose()
        {
            connection.Value.Result.Dispose();
        }

        public async Task<StreamEventsSlice> ReadStreamEventsForward(string stream, long start, int count, bool resolveLinkTos)
        {
            return await (await connection.Value).ReadStreamEventsForwardAsync(stream, start, count, resolveLinkTos);
        }

        private void Reconnect()
        {
            connection = new Lazy<Task<IEventStoreConnection>>(InitializeConnection);
        }

        private async Task<IEventStoreConnection> InitializeConnection()
        {
            var conn = EventStoreConnection.Create(connectionString);
            conn.Closed += (sender, args) => Reconnect();
            await conn.ConnectAsync();
            return conn;
        }
    }
}
