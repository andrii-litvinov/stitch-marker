using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using EventStore.ClientAPI;
using Google.Protobuf;
using Newtonsoft.Json.Linq;

namespace Service
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
    }
}
