﻿using System.Threading.Tasks;
using Proto;

namespace Service.Patterns.Xsd
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
                    var pattern = patternReader.Read(command.Content);
                    pattern.Id = command.Id;
                    pattern.Info.Title = command.FileName;
                    pattern.OwnerId = command.OwnerId;
                    var @event = new PatternCreated {SourceId = pattern.Id, Pattern = pattern, OwnerId = pattern.OwnerId};
                    context.Send(context.Parent, @event);
                    break;
                case ReceiveTimeout _:
                    context.Self.Stop();
                    break;
            }

            return Actor.Done;
        }
    }
}
