using System.Collections.Generic;
using System.Threading.Tasks;
using Proto;
using SM.Service.Messages;

namespace SM.Service.UserPatterns
{
    public class UserPatternsActor : IActor
    {
        private readonly List<string> patternIds;

        public UserPatternsActor()
        {
            patternIds = new List<string>();
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
                case GetUserPatternsMessage _:
                    // var (user, _) = await Cluster.GetAsync($"auth0|5ad7b04a5fb3bc1b6d7a98d8", "user");
                    // var response = await user.RequestAsync<GetUserPatternsMessage>(new GetUserPatternsMessage(), 10.Seconds());
                    context.Sender.Tell(new GetUserPatternsMessage
                    {
                        PatternIds = {patternIds}
                    });
                    break;
                case AddUserPatternMessage m:
                    if (!patternIds.Contains(m.MessageId)) patternIds.Add(m.MessageId);
                    break;
                default:
                    break;
            }
        }
    }
}
