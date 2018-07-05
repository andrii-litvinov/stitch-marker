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
                    context.Sender.Tell(new GetUserPatternsMessage
                    {
                        PatternIds = {patternIds}
                    });
                    break;
                case AddUserPatternMessage m:
                    if (!patternIds.Contains(m.PatternId)) patternIds.Add(m.PatternId);
                    break;
                case DeleteUserPatternMessage m:
                    if (!patternIds.Contains(m.PatternId)) patternIds.Remove(m.PatternId);
                    break;
                default:
                    break;
            }
        }
    }
}
