using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace SM.Service.Infrastructure.EventStore
{
    public interface ISubscriptionEventStoreConnection
    {
        Task<EventStoreSubscription> SubscribeToStream(string stream, bool resolveLinkTos,
            Func<EventStoreSubscription, ResolvedEvent, Task> eventAppeared,
            Action<EventStoreSubscription, SubscriptionDropReason, Exception> subscriptionDropped = null,
            UserCredentials userCredentials = null);
    }
}
