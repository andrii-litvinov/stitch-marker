using System;

namespace SM.Service.Messages
{
    public class PatternPreview
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public uint Height { get; set; }
        public uint Width { get; set; }
    }
}
