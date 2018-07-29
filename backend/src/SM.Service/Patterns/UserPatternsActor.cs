using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proto;
using SM.Service.Messages;

namespace SM.Service.UserPatterns
{
    public class UserPatternsActor : IActor
    {
        private readonly List<PatternBaseInfo> patternInfos;

        public UserPatternsActor()
        {
            patternInfos = new List<PatternBaseInfo>();
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    break;
                case PatternCreated m:
                    patternInfos.Add(new PatternBaseInfo
                    {
                        Author = m.Pattern.Info.Author,
                        Company = m.Pattern.Info.Company,
                        Copyright = m.Pattern.Info.Copyright,
                        Title = m.Pattern.Info.Title,
                        Height = m.Pattern.Height,
                        Width = m.Pattern.Width,
                        Id = m.Pattern.Id
                    });
                    break;
                case PatternDeleted m:
                    var patternInfo = patternInfos.FirstOrDefault(p => p.Id == m.Id);
                    if (patternInfo != null) patternInfos.Remove(patternInfo);
                    break;
                case GetPatternsInfo _:
                    context.Parent.Tell(new UserPatternsInfo
                    {
                        OwnerId = context.Self.Id,
                        PatternBaseInfo = {patternInfos}
                    });
                    break;
            }
        }
    }
}
