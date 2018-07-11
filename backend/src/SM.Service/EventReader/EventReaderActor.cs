using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Proto;
using Proto.Cluster;
using SM.Service.Extensions;
using SM.Service.Infrastructure.EventStore;

namespace SM.Service.EventReader
{
    public class EventReaderActor : IActor
    {
        private readonly ISubscriptionEventStoreConnection connection;
        private Position? lastPosition;

        public EventReaderActor(ISubscriptionEventStoreConnection subscriptionEventStoreConnection)
        {
            connection = subscriptionEventStoreConnection;
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    SubscribeToAllEvents(Position.Start);
                    break;
                case ReceiveTimeout _:
                    context.Self.Stop();
                    break;
                default:
                    break;
            }
        }

        private void SubscribeToAllEvents(Position position)
        {
            var settings = new CatchUpSubscriptionSettings(100, 100, false, false, "");
            connection.SubscribeToAllFrom(position, settings, EventAppeared, null, SubscriptionDropped, null);
        }

        private void SubscriptionDropped(EventStoreCatchUpSubscription eventStoreCatchUpSubscription, SubscriptionDropReason subscriptionDropReason,
            Exception exception)
        {
            SubscribeToAllEvents(lastPosition.Value);
        }

        private void EventAppeared(EventStoreCatchUpSubscription eventStoreCatchUpSubscription, ResolvedEvent resolvedEvent)
        {
            lastPosition = resolvedEvent.OriginalPosition;
            if (resolvedEvent.OriginalStreamId.StartsWith("$")) return;

            var message = resolvedEvent.Event.ReadMessage();

            if (message is IOwnerId ownerId)
            {
                var (user, _) = Cluster.GetAsync($"user-{ownerId.GetOwnerId}", "user").Result;
                user.Tell(message);
            }
        }
    }
}
