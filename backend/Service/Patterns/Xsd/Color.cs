using System.Collections.Generic;

namespace SM.Service.Patterns.Xsd
{
    public class Color
    {
        public readonly List<BlendColor> Blends = new List<BlendColor>();
        public string ColorCode = "";
        public string ColorTitle = "";
        public string Description;
        public string FontFamily = "";
        public ItemType HasItems;
        public Rgb Rgb;
        public Strands Strands = default;
        public string Symbol = "";
        public string VendorCode = "";
        public string VendorTitle = "";

        public Color(int index)
        {
            Index = index;
        }

        public int Index { get; }

        public class BlendColor
        {
            public string ColorCode = "";
            public string ColorTitle = "";
            public Rgb Rgb;
            public Strands Strands = default;
            public string VendorCode = "";
            public string VendorTitle = "";
        }
    }
}
