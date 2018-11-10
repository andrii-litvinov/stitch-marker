using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Google.Protobuf;
using Proto.Persistence;

namespace Service
{
    public class EventStore : IEventStore
    {
        private readonly IReadWriteEventStoreConnection connection;

        public EventStore(IReadWriteEventStoreConnection connection)
        {
            this.connection = connection;
        }

        public async Task<long> GetEventsAsync(string actorName, long indexStart, long indexEnd,
            Action<object> callback)
        {
            StreamEventsSlice slice;
            var start = indexStart;

            do
            {
                var count = (int) Math.Min(indexEnd - start, 199) + 1;
                slice = await connection.ReadStreamEventsForward(actorName, start, count, false);

                foreach (var resolvedEvent in slice.Events)
                    callback(resolvedEvent.Event.ToMessage());

                start = slice.NextEventNumber;
            } while (start <= indexEnd && !slice.IsEndOfStream);

            return slice.NextEventNumber;
        }

        public async Task<long> PersistEventAsync(string actorName, long index, object @event)
        {
            switch (@event)
            {
                case IMessage message:
                    var result = await connection.AppendToStream(actorName, index - 1, message.ToEventData());
                    return result.NextExpectedVersion;
                default:
                    throw new Exception($"Expected event of type 'IMessage', but found {@event.GetType().FullName}.");
            }
        }

        public Task DeleteEventsAsync(string actorName, long inclusiveToIndex)
        {
            throw new NotSupportedException();
        }
    }
}
