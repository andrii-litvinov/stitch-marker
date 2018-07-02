using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace SM.Service.Infrastructure.EventStore
{
    public interface ISubscriptionEventStoreConnection
    {
        Task<AllEventsSlice> ReadAllEventsForwardAsync(Position position, int maxCount, bool resolveLinkTos);

        Task<EventStoreSubscription> SubscribeToAllAsync(bool resolveLinkTos, Func<EventStoreSubscription, ResolvedEvent, Task> eventAppeared,
            Action<EventStoreSubscription, SubscriptionDropReason, Exception> subscriptionDropped = null, UserCredentials userCredentials = null);
    }
}
