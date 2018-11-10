using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Proto;

namespace Service.Patterns
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

        private void LiveProcessingStarted(EventStoreCatchUpSubscription subscription) =>
            context.Send(context.Parent, new LiveProcessingStarted());

        private void SubscriptionDropped(
            EventStoreCatchUpSubscription eventStoreCatchUpSubscription,
            SubscriptionDropReason subscriptionDropReason,
            Exception exception) => Subscribe();

        private async Task EventAppeared(EventStoreCatchUpSubscription eventStoreCatchUpSubscription, ResolvedEvent resolvedEvent)
        {
            // TODO [AL]: Handle and log exceptions if any.
            if (!resolvedEvent.OriginalStreamId.StartsWith("$"))
            {
                var message = resolvedEvent.Event.ToMessage();
                if (message != null) context.Send(context.Parent, message);
            }

            position = resolvedEvent.OriginalPosition;
        }
    }
}
