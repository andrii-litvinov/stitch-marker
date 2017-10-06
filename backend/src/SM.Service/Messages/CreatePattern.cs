using System;

namespace SM.Service.Messages
{
    public class CreatePattern
    {
        public CreatePattern(Guid id, string fileName, byte[] content)
        {
            Id = id;
            FileName = fileName;
            Content = content;
        }

        public Guid Id { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
    }
}
