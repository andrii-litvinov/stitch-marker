using System;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace SM.Service.Infrastructure.EventStore
{
    public interface ISubscriptionEventStoreConnection
    {
        EventStoreAllCatchUpSubscription SubscribeToAllFrom(Position? lastCheckpoint, CatchUpSubscriptionSettings settings,
            Action<EventStoreCatchUpSubscription, ResolvedEvent> eventAppeared, Action<EventStoreCatchUpSubscription> liveProcessingStarted = null,
            Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception> subscriptionDropped = null, UserCredentials userCredentials = null);
    }
}
