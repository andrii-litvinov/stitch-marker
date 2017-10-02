using System.Threading.Tasks;
using Proto;
using SM.Core;
using SM.Core.Model;
using SM.Xsd;

namespace SM.Service.Classes
{
    public class XsdPatternParser : IActor
    {
        private readonly IPatternReader patternReader = new XsdPatternReader();
        private PatternState pattern;

        public Task ReceiveAsync(IContext context)
        {
            if (context.Message is CreatePattern command)
            {
                pattern = patternReader.Read(command.Content);
                pattern.Info.Title = command.FileName;
                pattern.PatternId = command.PatternId;
            }
            context.Parent.Tell(pattern);

            return Actor.Done;
        }
    }
}
