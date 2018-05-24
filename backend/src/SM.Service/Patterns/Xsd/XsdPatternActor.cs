using System.Threading.Tasks;
using Proto;
using SM.Service.Extensions;
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
                case Started _:
                    context.SetReceiveTimeout(5.Minutes());
                    break;
                case CreatePattern command:
                    var pattern = patternReader.Read(command.Content.ToByteArray());
                    pattern.Id = command.Id;
                    pattern.Info.Title = command.FileName;
                    pattern.UserId = command.UserId;
                    var @event = new PatternCreated { Id = pattern.Id, Pattern = pattern };
                    context.Parent.Tell(@event);
                    break;
                case ReceiveTimeout _:
                    context.Self.Stop();
                    break;
            }
            return Actor.Done;
        }
    }
}
