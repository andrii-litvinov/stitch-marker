using System.Threading.Tasks;
using Proto;
using SM.Service.Messages;

namespace SM.Service.Patterns.Xsd
{
    public class XsdPatternActor : IActor
    {
        private readonly XsdPatternReader patternReader = new XsdPatternReader();

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case CreatePattern command:
                    var pattern = patternReader.Read(command.Content.ToByteArray());
                    pattern.Info.Title = command.FileName;
                    pattern.Id = command.Id.ToString();
                    context.Parent.Tell(pattern);
                    break;
            }
            return Actor.Done;
        }
    }
}
