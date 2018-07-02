using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Proto.Persistence;

namespace SM.Service.Infrastructure.EventStore
{
    public class AllEventsReader : IEventStore
    {
        private readonly ISubscriptionEventStoreConnection connection;

        public AllEventsReader(ISubscriptionEventStoreConnection connection)
        {
            this.connection = connection;

            Subscribe();

            var events = ReadAllEvents();
        }

        private async void Subscribe()
        {
            var result = await connection.SubscribeToAllAsync(false, EventAppeared);
        }

        private Task EventAppeared(EventStoreSubscription arg1, ResolvedEvent arg2)
        {
            return null;
        }

        public List<ResolvedEvent> ReadAllEvents()
        {
            var allEvents = new List<ResolvedEvent>();

            AllEventsSlice currentSlice;
            var nextSliceStart = Position.Start;

            do
            {
                currentSlice = connection.ReadAllEventsForwardAsync(nextSliceStart, 200, false).Result;

                nextSliceStart = currentSlice.NextPosition;

                foreach (var currentSliceEvent in currentSlice.Events)
                    if (currentSliceEvent.OriginalStreamId.StartsWith("pattern"))
                        allEvents.Add(currentSliceEvent);
            } while (!currentSlice.IsEndOfStream);

            return allEvents;
        }

        public Task<long> GetEventsAsync(string actorName, long indexStart, long indexEnd, Action<object> callback)
        {
            throw new NotImplementedException();
        }

        public Task<long> PersistEventAsync(string actorName, long index, object @event)
        {
            throw new NotImplementedException();
        }

        public Task DeleteEventsAsync(string actorName, long inclusiveToIndex)
        {
            throw new NotImplementedException();
        }
    }
}
