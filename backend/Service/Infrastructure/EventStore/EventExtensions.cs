﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EventStore.ClientAPI;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Service
{
    using Cache = Lazy<Dictionary<string, Lazy<Func<IMessage>>>>;

    public static class EventExtensions
    {
        private static readonly Cache Types = new Cache(() => typeof(PatternCreated).Assembly
            .GetTypes()
            .Where(type => typeof(IMessage).IsAssignableFrom(type))
            .ToDictionary(
                type => type.Name,
                type => new Lazy<Func<IMessage>>(
                    () => Expression.Lambda<Func<IMessage>>(Expression.New(type.GetConstructor(Array.Empty<Type>()))).Compile())));

        public static IMessage ToMessage(this RecordedEvent recordedEvent)
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

            if (Types.Value.TryGetValue(recordedEvent.EventType, out var factory))
            {
                var message = factory.Value.Invoke();
                message.MergeFrom(data);
                return message;
            }

            return null;
        }

        public static EventData ToEventData(this IMessage message)
        {
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
                    metadata = Encoding.UTF8.GetBytes(new JObject {["encoding"] = "gzip"}.ToString(Formatting.None));
                }

            var eventData = new EventData(Guid.NewGuid(), message.GetType().Name, false, data, metadata);
            return eventData;
        }
    }
}
