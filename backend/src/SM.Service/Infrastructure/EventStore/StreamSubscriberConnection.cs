using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace SM.Service.Infrastructure.EventStore
{
    public class StreamSubscriberConnection : ISubscriptionEventStoreConnection
    {
        private readonly string connectionString;
        private Lazy<Task<IEventStoreConnection>> connection;

        public StreamSubscriberConnection(string connectionString)
        {
            this.connectionString = connectionString;
            Reconnect();
        }

        public async Task<AllEventsSlice> ReadAllEventsForwardAsync(Position position, int maxCount, bool resolveLinkTos)
        {
            return await (await connection.Value).ReadAllEventsForwardAsync(position, 200, false);
        }

        public async Task<EventStoreSubscription> SubscribeToAllAsync(bool resolveLinkTos, Func<EventStoreSubscription, ResolvedEvent, Task> eventAppeared,
            Action<EventStoreSubscription, SubscriptionDropReason, Exception> subscriptionDropped = null, UserCredentials userCredentials = null)
        {
            return await (await connection.Value).SubscribeToAllAsync(resolveLinkTos, eventAppeared, subscriptionDropped, userCredentials);
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

        public void Dispose()
        {
            connection.Value.Result.Dispose();
        }
    }
}
