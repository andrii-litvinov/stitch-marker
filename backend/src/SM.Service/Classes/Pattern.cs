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
            if (context.Message is CreatePattern pattern)
                using (var memoryStream = new MemoryStream(pattern.Content) {Position = 0})
                {
                    state = patternReader.Read(memoryStream);
                }
            else if (context.Message is PatternQuery)
                context.Respond(state);
            return Actor.Done;
        }
    }
}