using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Proto;
using Proto.Cluster;
using SM.Service.Extensions;
using SM.Service.Infrastructure.EventStore;
using SM.Service.Messages;
using SM.Service.UserPatterns;

namespace SM.Service.EventReader
{
    public class EventReaderActor : IActor
    {
        private readonly ISubscriptionEventStoreConnection connection;
        private List<ResolvedEvent> events;

        public EventReaderActor(ISubscriptionEventStoreConnection subscriptionEventStoreConnection)
        {
            connection = subscriptionEventStoreConnection;

            var result = connection.SubscribeToAllAsync(false, EventAppeared).Result;
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    var createdList = FilterEvents(ReadAllEvents());
                    foreach (var resolvedEvent in createdList)
                    {
                        var (pattern, _) = await Cluster.GetAsync($"{resolvedEvent.OriginalStreamId}", "pattern");
                        var response = await pattern.RequestAsync<PatternOwner>(new GetPatternOwner {Id = resolvedEvent.OriginalStreamId}, 10.Seconds());
                        var (user, _) = await Cluster.GetAsync($"{response.OwnerId}", "user");

                        if (user == null)
                        {
                            var props = Actor.FromProducer(() => new UserPatternsActor());
                            Actor.SpawnNamed(props, response.OwnerId);
                            (user, _) = await Cluster.GetAsync($"{response.OwnerId}", "user");
                        }

                        user.Tell(new AddUserPatternMessage
                        {
                            MessageId = resolvedEvent.OriginalStreamId
                        });
                    }

                    break;
                case ReceiveTimeout _:
                    context.Self.Stop();
                    break;
                default:
                    break;
            }
        }

        private List<ResolvedEvent> FilterEvents(List<ResolvedEvent> resolvedEvents)
        {
            var d = resolvedEvents.Where(i => i.OriginalEvent.EventType == "PatternDeleted").ToList();
            var c = resolvedEvents.Where(i => i.OriginalEvent.EventType == "PatternCreated").ToList();

            resolvedEvents.Clear();

            foreach (var item in c)
                if (d.Where(i => i.OriginalStreamId.Equals(item.OriginalStreamId)).ToList().Count == 0)
                    resolvedEvents.Add(item);

            return resolvedEvents;
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
