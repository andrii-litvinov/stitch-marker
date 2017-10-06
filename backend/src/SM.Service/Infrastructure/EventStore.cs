using System;
using System.Threading.Tasks;
using Proto.Persistence;

namespace SM.Service.Infrastructure
{
    public class EventStore : IEventStore
    {
        public Task<long> GetEventsAsync(string actorName, long indexStart, long indexEnd, Action<object> callback)
        {
            return Task.FromResult(0L);
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
