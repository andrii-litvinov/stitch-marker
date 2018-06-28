using EventStore.ClientAPI;
using Proto.Persistence;
using System;
using System.Threading.Tasks;

namespace SM.Service.Infrastructure.EventStore
{
    public class AllEventsReader : IEventStore
    {
        private readonly ISubscriptionEventStoreConnection connection;

        public AllEventsReader(ISubscriptionEventStoreConnection connection)
        {
            this.connection = connection;
            Start();

        }

        private async void Start()
        {
            var res = await connection.SubscribeToStream("$all", true, EventAppeared);
        }

        private Task EventAppeared(EventStoreSubscription arg1, ResolvedEvent arg2)
        {
            throw new NotImplementedException();
        }

        public async Task<long> GetEventsAsync(string actorName, long indexStart, long indexEnd,
                Action<object> callback)
        {
            return 0;
        }

        public async Task<long> PersistEventAsync(string actorName, long index, object @event)
        {
            return 0;
        }

        public Task DeleteEventsAsync(string actorName, long inclusiveToIndex)
        {
            throw new NotSupportedException();
        }
    }
}
