using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Proto.Persistence;

namespace SM.Service.Infrastructure.EventStore
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
                {
                    var data = resolvedEvent.Event.Data;
                    if (resolvedEvent.Event.Metadata.Length > 0)
                    {
                        var metadata = JObject.Parse(Encoding.UTF8.GetString(resolvedEvent.Event.Metadata));
                        if (metadata["encoding"].Value<string>() == "gzip")
                            using (var originalStream = new MemoryStream())
                            {
                                using (var compressedStream = new MemoryStream(data))
                                using (var gZipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                                {
                                    gZipStream.CopyTo(originalStream);
                                }

                                data = originalStream.ToArray();
                            }
                    }

                    var eventType = Type.GetType($"SM.Service.Messages.{resolvedEvent.Event.EventType}");
                    var message = (IMessage) Activator.CreateInstance(eventType);

                    message.MergeFrom(data);
                    callback(message);
                }
                
                start = slice.NextEventNumber;
            } while (start <= indexEnd && !slice.IsEndOfStream);

            return slice.NextEventNumber;
        }

        public async Task<long> PersistEventAsync(string actorName, long index, object @event)
        {
            switch (@event)
            {
                case IMessage message:
                    var data = message.ToByteArray();
                    var metadata = Array.Empty<byte>();

                    if (data.Length > 512 * 1024)
                        using (var compressedStream = new MemoryStream())
                        {
                            using (var originalStream = new MemoryStream(data))
                            using (var gZipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                            {
                                originalStream.CopyTo(gZipStream);
                            }

                            data = compressedStream.ToArray();
                            metadata = Encoding.UTF8.GetBytes(
                                new JObject {["encoding"] = "gzip"}.ToString(Formatting.None));
                        }

                    var eventData = new EventData(Guid.NewGuid(), @event.GetType().Name, false, data, metadata);
                    var result = await connection.AppendToStream(actorName, index - 1, eventData);
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
