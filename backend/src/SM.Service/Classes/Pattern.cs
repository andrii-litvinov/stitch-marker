using System.IO;
using System.Threading.Tasks;
using Proto;
using SM.Core;
using SM.Core.Model;

namespace SM.Service.Classes
{
    public class Pattern : IActor
    {
        private readonly IPatternReader patternReader;
        private PatternState state;

        public Pattern(IPatternReader patternReader)
        {
            this.patternReader = patternReader;
        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case CreatePattern pattern:
                    state = patternReader.Read(pattern.Content);
                    break;
                case PatternQuery _:
                    context.Respond(state);
                    break;
            }
            
            return Actor.Done;
        }
    }
}
