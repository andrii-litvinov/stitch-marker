using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using Proto;
using Proto.Cluster;
using SM.Service.Infrastructure.EventStore;

namespace SM.Service.EventReader
{
    public class EventReaderActor : IActor
    {
        private readonly ISubscriptionEventStoreConnection connection;

        public EventReaderActor(ISubscriptionEventStoreConnection subscriptionEventStoreConnection)
        {
            connection = subscriptionEventStoreConnection;
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    SubscribeToAllEvents();
                    break;
                case ReceiveTimeout _:
                    context.Self.Stop();
                    break;
                default:
                    break;
            }
        }

        private void SubscribeToAllEvents()
        {
            var settings = new CatchUpSubscriptionSettings(10, 500, false, false, "");
            connection.SubscribeToAllFrom(Position.Start, settings, EventAppeared, LiveProcessingStarted, SubscriptionDropped, null);
        }

        private void SubscriptionDropped(EventStoreCatchUpSubscription eventStoreCatchUpSubscription, SubscriptionDropReason subscriptionDropReason,
            Exception exception)
        {
        }

        private void LiveProcessingStarted(EventStoreCatchUpSubscription eventStoreCatchUpSubscription)
        {
        }

        private void EventAppeared(EventStoreCatchUpSubscription eventStoreCatchUpSubscription, ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.OriginalStreamId.StartsWith("$")) return;

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
            var instance = Activator.CreateInstance(eventType);
            var message = (IMessage) instance;

            message.MergeFrom(data);

            var ownerId = message as IOwnerId;
            if (ownerId != null)
            {
                var (user, _) = Cluster.GetAsync($"user-{ownerId}", "user").Result;
                user.Tell(message);
            }
        }
    }
}
