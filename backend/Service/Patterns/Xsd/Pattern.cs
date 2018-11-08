using System;
using System.Collections.Generic;

namespace Service.Patterns.Xsd
{
    public class Pattern
    {
        public List<Backstitch> Backstitches = new List<Backstitch>();
        public readonly Canvas Canvas = new Canvas();
        public readonly List<Color> Colors = new List<Color>();
        public ushort Height;
        public Info Info = default;
        public List<Node> Nodes = new List<Node>();
        public readonly List<Stitch> Stitches = new List<Stitch>();
        public Strands Strands = default;
        public ushort Width;

        public ushort ThreadCountX { get; set; }
        public ushort ThreadCountY { get; set; }
    }

    [Flags]
    public enum ItemType
    {
        None,
        Full,
        Half,
        Quarter = 4,
        ThreeQuarter = 8,
        Petit = 16,
        FrenchKnot = 32,
        BackStitch = 64,
        Bead = 128
    }

    public struct Strands
    {
        public int Full;
        public int Half;
        public int Quarter;
        public int ThreeQuarter;
        public int Petit;
        public int BackStitch;
        public int FrenchKnot;
    }

    public class TypeCounts
    {
        public int BackStitch;
        public int Bead;
        public int FrenchKnot;
        public int Full;
        public int Half;
        public int Petit;
        public int Quarter;
        public int ThreeQuarter;
    }

    public struct Info
    {
        public string Title;
        public string Author;
        public string Company;
        public string Copyright;
    }

    public class Canvas
    {
        public Rgb CustomRGB;
        public Rgb DefaultRGB;
        public string Title;
        public bool UseCustomRGB;
    }
}
