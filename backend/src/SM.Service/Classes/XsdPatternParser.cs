using System.Threading.Tasks;
using Proto;
using SM.Core;
using SM.Xsd;

namespace SM.Service.Classes
{
    public class XsdPatternParser : IActor
    {
        private readonly IPatternReader patternReader = new XsdPatternReader();

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case CreatePattern command:
                    var pattern = patternReader.Read(command.Content);
                    pattern.Info.Title = command.FileName;
                    pattern.Id = command.Id;
                    context.Parent.Tell(new PatternParsed {Id = command.Id, Pattern = pattern});
                    break;
            }
            return Actor.Done;
        }
    }
}
