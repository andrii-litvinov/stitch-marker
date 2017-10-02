using System;
using System.Collections.Generic;

namespace SM.Core.Model
{
    public class PatternState
    {
        public Guid Id { get; set; }
        public Info Info { get; } = new Info();
        public Canvas Canvas { get; } = new Canvas();
        public StrandsCount StrandsCount { get; } = new StrandsCount();
        public uint Width { get; set; }
        public uint Height { get; set; }
        public List<StitchConfiguration> Configurations { get; } = new List<StitchConfiguration>();
        public List<Stitch> Stitches { get; } = new List<Stitch>();
        public List<Backstitch> Backstitches { get; } = new List<Backstitch>();
        public List<Element> Elements { get; } = new List<Element>();
    }
}
