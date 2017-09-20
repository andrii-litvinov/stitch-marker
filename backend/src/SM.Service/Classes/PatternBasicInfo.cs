using System;

namespace SM.Service.Classes
{
    public class PatternBasicInfo
    {
        public Guid PatternId { get; set; }
        public string PatternName { get; set; }
        public uint Height { get; set; }
        public uint Width { get; set; }
        public string Image { get; set; }
    }
}
