using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using Proto.Cluster;

namespace SM.Service
{
    public static class EventStoreExtensions
    {
        public static IMessage ReadMessage(this RecordedEvent recordedEvent)
        {
            var data = recordedEvent.Data;
            if (recordedEvent.Metadata.Length > 0)
            {
                var metadata = JObject.Parse(Encoding.UTF8.GetString(recordedEvent.Metadata));
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

            var eventType = Type.GetType($"SM.Service.{recordedEvent.EventType}");
            if (eventType != null)
            {
                var instance = Activator.CreateInstance(eventType);
                if (instance is IMessage message)
                {
                    message.MergeFrom(data);
                    return message;
                }
            }

            return null;
        }

        public static async Task<PatternItems> GetUserPatterns(this string userId)
        {
            var (patternsByOwnerProjection, _) = await Cluster.GetAsync(ActorKind.PatternsByOwnerProjection, ActorKind.PatternsByOwnerProjection);

            var query = new GetPatternItems {RequestId = Guid.NewGuid().ToString(), OwnerId = userId, Skip = 0, Take = 100};

            while (true)
            {
//                if (some timeout condition) throw new TimeoutException();
                var response = await patternsByOwnerProjection.RequestAsync<object>(query, 10.Seconds());
                switch (response)
                {
                    case PatternItems items:
                        return items;
                    case CatchingUp _:
                        await Task.Delay(100);
                        break;
                    default:
                        throw new Exception("Unknown response type.");
                }
            }
        }
    }
}
