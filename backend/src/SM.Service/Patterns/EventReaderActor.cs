using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Proto;
using SM.Service.Extensions;
using SM.Service.Infrastructure.EventStore;

namespace SM.Service.Patterns
{
    public class EventSubscriptionActor : IActor
    {
        private readonly ISubscriptionEventStoreConnection connection;
        private IContext context;
        private Position? position = Position.Start;

        public EventSubscriptionActor(ISubscriptionEventStoreConnection connection) => this.connection = connection;

        public async Task ReceiveAsync(IContext ctx)
        {
            switch (ctx.Message)
            {
                case Started _:
                    context = ctx;
                    Subscribe();
                    break;
            }
        }

        private void Subscribe() =>
            connection.SubscribeToAllFrom(
                position,
                new CatchUpSubscriptionSettings(100, 100, false, false),
                EventAppeared,
                LiveProcessingStarted,
                SubscriptionDropped);

        private void LiveProcessingStarted(EventStoreCatchUpSubscription subscription)
        {
            // TODO: Send response to parent actor that all existing events were read from store.  
        }

        private void SubscriptionDropped(
            EventStoreCatchUpSubscription eventStoreCatchUpSubscription,
            SubscriptionDropReason subscriptionDropReason,
            Exception exception) => Subscribe();

        private async Task EventAppeared(EventStoreCatchUpSubscription eventStoreCatchUpSubscription, ResolvedEvent resolvedEvent)
        {
            position = resolvedEvent.OriginalPosition;

            if (!resolvedEvent.OriginalStreamId.StartsWith("$"))
                context.Parent.Tell(resolvedEvent.Event.ReadMessage());
        }
    }
}
