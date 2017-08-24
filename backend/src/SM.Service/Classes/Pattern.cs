using System.IO;
using System.Threading.Tasks;
using Proto;
using SM.Core;

namespace SM.Service.Classes
{
    public class Pattern : IActor
    {
        private readonly IPatternReader patternReader;
        private Core.Model.Pattern state;

        public Pattern(IPatternReader patternReader)
        {
            this.patternReader = patternReader;
        }

        public Task ReceiveAsync(IContext context)
        {
            if (context.Message is CreatePattern pattern)
            {
                var patternBytes = pattern.Content;
                var memoryStream = new MemoryStream(patternBytes) {Position = 0};
                var patternRead = patternReader.Read(memoryStream);
                state = patternRead;
            }
            return Actor.Done;
        }
    }
}