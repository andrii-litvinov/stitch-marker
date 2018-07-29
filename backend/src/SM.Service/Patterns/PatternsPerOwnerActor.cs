using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Proto;
using SM.Service.Extensions;
using SM.Service.Messages;

namespace SM.Service.Patterns
{
    public class PatternsPerOwnerActor : IActor
    {
        private readonly Dictionary<string, string> ownerPatterns;
        private readonly Dictionary<string, string> patternOwner;
        private readonly MemoryCache senders;

        public PatternsPerOwnerActor()
        {
            ownerPatterns = new Dictionary<string, string>();
            patternOwner = new Dictionary<string, string>();
            senders = new MemoryCache(new MemoryCacheOptions());
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    context.GetChild<UserPatternsActor>();
                    break;
                case PatternCreated m:
                    var userPatternsName = $"user-{Guid.NewGuid()}";
                    ownerPatterns.TryAdd(ownerPatterns[m.OwnerId], userPatternsName);
                    patternOwner.TryAdd(m.Id, userPatternsName);

                    var user = context.GetChild<UserPatternsActor>(ownerPatterns[m.OwnerId]);
                    user.Tell(m);
                    break;
                case PatternDeleted m:
                    user = context.GetChild<UserPatternsActor>(patternOwner[m.Id]);
                    user.Tell(m);
                    break;
                case GetPatternItems m:
                    senders.Set(m.RequestId, context.Sender, 30.Seconds());
                    user = context.GetChild<UserPatternsActor>(ownerPatterns[m.OwnerId]);
                    user.Tell(m);
                    break;
                case PatternItems m:
                    senders.Get<PID>(patternOwner[m.RequestId])?.Tell(m);
                    break;
            }
        }
    }
}
