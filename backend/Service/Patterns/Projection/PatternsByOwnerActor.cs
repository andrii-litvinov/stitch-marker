using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proto;

namespace Service.Patterns
{
    public class PatternsByOwnerActor : IActor
    {
        private readonly Dictionary<string, PatternItem> patterns = new Dictionary<string, PatternItem>();

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    break;
                case PatternCreated m:
                    if (m.Pattern != null) // TODO: Remove after EventStore cleanup.
                        patterns[m.SourceId] = new PatternItem
                        {
                            Author = m.Pattern.Info.Author,
                            Company = m.Pattern.Info.Company,
                            Copyright = m.Pattern.Info.Copyright,
                            Title = m.Pattern.Info.Title,
                            Height = m.Pattern.Height,
                            Width = m.Pattern.Width,
                            Id = m.Pattern.Id
                        };

                    break;
                case PatternDeleted m:
                    patterns.Remove(m.SourceId);
                    break;
                case GetPatternItems m:
                    context.Send(context.Parent, new PatternItems {RequestId = m.RequestId, Items = {patterns.Values.Skip(m.Skip).Take(m.Take)}});
                    break;
            }
        }
    }
}
