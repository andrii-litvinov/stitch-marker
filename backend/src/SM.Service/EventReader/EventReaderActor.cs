using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Proto;
using SM.Service.Infrastructure.EventStore;

namespace SM.Service.EventReader
{
    public class EventReaderActor : IActor
    {
        private readonly ISubscriptionEventStoreConnection connection;
        private List<ResolvedEvent> events;

        public EventReaderActor(ISubscriptionEventStoreConnection subscriptionEventStoreConnection)
        {
            connection = subscriptionEventStoreConnection;

            events = ReadAllEvents();

            var result = connection.SubscribeToAllAsync(false, EventAppeared).Result;
        }

        public Task ReceiveAsync(IContext context)
        {
            return null;
        }

        private List<ResolvedEvent> ReadAllEvents()
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

        private Task EventAppeared(EventStoreSubscription arg1, ResolvedEvent arg2)
        {
            return null;
        }
    }
}
