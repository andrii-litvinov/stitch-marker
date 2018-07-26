using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Proto;
using Proto.Cluster;
using SM.Service.Extensions;
using SM.Service.Infrastructure.EventStore;
using SM.Service.PatternsManager;

namespace SM.Service.EventReader
{
    public class EventReaderActor : IActor
    {
        private readonly ISubscriptionEventStoreConnection connection;
        private Position? lastPosition;
        private IContext context;

        public EventReaderActor(ISubscriptionEventStoreConnection subscriptionEventStoreConnection)
        {
            connection = subscriptionEventStoreConnection;
            lastPosition = Position.Start;
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    this.context = context;
                    context.Spawn(Actor.FromProducer(() => new PatternsManagerActor()));
                    Subscribe();
                    break;
            }
        }

        private void Subscribe()
        {
            connection.SubscribeToAllFrom(lastPosition, new CatchUpSubscriptionSettings(100, 100, false, false, ""), EventAppeared, null, SubscriptionDropped,
                null);
        }

        private void SubscriptionDropped(EventStoreCatchUpSubscription eventStoreCatchUpSubscription, SubscriptionDropReason subscriptionDropReason,
            Exception exception)
        {
            Subscribe();
        }

        private async Task EventAppeared(EventStoreCatchUpSubscription eventStoreCatchUpSubscription, ResolvedEvent resolvedEvent)
        {
            lastPosition = resolvedEvent.OriginalPosition;

            if (resolvedEvent.OriginalStreamId.StartsWith("$")) return;

            var message = resolvedEvent.Event.ReadMessage();

            var patternsManager = context.GetChild<PatternsManagerActor>();
            patternsManager.Tell(message);
        }
    }
}
