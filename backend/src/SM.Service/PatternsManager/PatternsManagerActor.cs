using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proto;
using Proto.Cluster;
using SM.Service.Extensions;
using SM.Service.Messages;

namespace SM.Service.PatternsManager
{
    public class PatternsManagerActor : IActor
    {
        private readonly Dictionary<string, string> ownerIds;

        public PatternsManagerActor()
        {
            ownerIds = new Dictionary<string, string>();
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    break;
                case ReceiveTimeout _:
                    context.Self.Stop();
                    break;
                case PatternCreated m:
                    if (!ownerIds.ContainsKey(m.OwnerId))
                        ownerIds.Add(m.OwnerId, $"user-{Guid.NewGuid()}");

                    var (user, _) = await Cluster.GetAsync(ownerIds[m.OwnerId], "user");
                    user.Tell(m);
                    break;
                case PatternDeleted m:
                    (user, _) = await Cluster.GetAsync(ownerIds[m.OwnerId], "user");
                    user.Tell(m);
                    break;
                case GetPatterns m:
                    if (!ownerIds.ContainsKey(m.OwnerId))
                        ownerIds.Add(m.OwnerId, $"user-{Guid.NewGuid()}");
                    (user, _) = await Cluster.GetAsync(ownerIds[m.OwnerId], "user");

                    var patterns = await user.RequestAsync<GetPatterns>(new GetPatterns(), 10.Seconds());

                    context.Sender.Tell(new GetPatterns
                    {
                        Patterns = {patterns.Patterns}
                    });
                    break;
            }
        }
    }
}
