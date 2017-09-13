using System;

namespace SM.Service.Classes
{
    public class CreatePattern
    {
        public CreatePattern(Guid patternId, string fileName, byte[] content)
        {
            PatternId = patternId;
            FileName = fileName;
            Content = content;
        }

        public Guid PatternId { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
    }
}
