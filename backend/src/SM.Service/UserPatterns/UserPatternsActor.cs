using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proto;
using SM.Service.Messages;

namespace SM.Service.UserPatterns
{
    public class UserPatternsActor : IActor
    {
        private readonly List<Pattern> patterns;

        public UserPatternsActor()
        {
            patterns = new List<Pattern>();
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
                    patterns.Add(m.Pattern);
                    break;
                case PatternDeleted m:
                    var pattern = patterns.First(p => p.Id == m.OwnerId);
                    if (pattern != null) patterns.Remove(pattern);
                    break;
                case GetPatterns _:
                    context.Sender.Tell(new GetPatterns
                    {
                        Patterns = {patterns}
                    });
                    break;
            }
        }
    }
}
