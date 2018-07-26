using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Proto;
using Proto.Cluster;
using SM.Service.Extensions;
using SM.Service.Messages;
using SM.Service.UserPatterns;

namespace SM.Service.PatternsManager
{
    public class PatternsManagerActor : IActor
    {
        private readonly Dictionary<string, string> ownerIds;
        private readonly MemoryCache senders;

        public PatternsManagerActor()
        {
            ownerIds = new Dictionary<string, string>();
            senders = new MemoryCache(new MemoryCacheOptions());
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    break;
                case PatternCreated m:
                    if (!ownerIds.ContainsKey(m.OwnerId))
                    {
                        ownerIds.Add(m.OwnerId, $"user-{Guid.NewGuid()}");
                    }

                    var user = context.GetChild<UserPatternsActor>(ownerIds[m.OwnerId]);
                    user.Tell(m);
                    break;
                case PatternDeleted m:
                    user = context.GetChild<UserPatternsActor>(ownerIds[m.OwnerId]);
                    user.Tell(m);
                    break;
                case GetPatterns m:
                    senders.Set(m.Id, context.Sender, 30.Seconds());

                    user = context.GetChild<UserPatternsActor>(ownerIds[m.OwnerId]);
                    user.Tell(m);
                    break;
            }
        }
    }
}
