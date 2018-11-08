using System.Collections.Generic;

namespace Service.Patterns
{
    public class PatternAggregate
    {
        // TODO: [AL] Rename to Pattern.
        private readonly Dictionary<(uint, uint, uint, uint), Backstitch> backstitches = new Dictionary<(uint, uint, uint, uint), Backstitch>();
        private readonly List<StitchConfiguration> configuration = new List<StitchConfiguration>();
        private readonly List<Element> elements = new List<Element>();
        private readonly Dictionary<(uint, uint), Stitch> stitches = new Dictionary<(uint, uint), Stitch>();
        private Canvas canvas;
        private uint height;
        private string id;
        private Info info;
        private string ownerId;
        private Strands strands;
        private uint width;

        public void Apply(PatternCreated @event)
        {
            var pattern = @event.Pattern;

            id = pattern.Id;
            ownerId = pattern.OwnerId;
            width = pattern.Width;
            height = pattern.Height;
            canvas = new Canvas {Title = pattern.Canvas.Title};
            info = new Info
            {
                Title = pattern.Info.Title,
                Author = pattern.Info.Author,
                Company = pattern.Info.Company,
                Copyright = pattern.Info.Copyright
            };
            strands = new Strands
            {
                Backstitch = pattern.Strands.Backstitch,
                FrenchKnot = pattern.Strands.FrenchKnot,
                Full = pattern.Strands.Full,
                Half = pattern.Strands.Half,
                Petit = pattern.Strands.Petit,
                Quarter = pattern.Strands.Quarter,
                ThreeQuarter = pattern.Strands.ThreeQuarter
            };

            foreach (var config in pattern.Configurations)
                configuration.Add(new StitchConfiguration
                {
                    Symbol = config.Symbol,
                    HexColor = config.HexColor,
                    Strands = config.Strands
                });

            foreach (var stitch in pattern.Stitches)
                stitches.Add((stitch.X, stitch.Y), new Stitch
                {
                    X = stitch.X,
                    Y = stitch.Y,
                    Type = stitch.Type,
                    ConfigurationIndex = stitch.ConfigurationIndex
                });

            foreach (var backstitch in pattern.Backstitches)
                backstitches.Add((backstitch.X1, backstitch.Y1, backstitch.X2, backstitch.Y2), new Backstitch
                {
                    X1 = backstitch.X1,
                    Y1 = backstitch.Y1,
                    X2 = backstitch.X2,
                    Y2 = backstitch.Y2,
                    ConfigurationIndex = backstitch.ConfigurationIndex
                });

            foreach (var element in pattern.Elements)
                elements.Add(new Element
                {
                    X = element.X,
                    Y = element.Y,
                    Type = element.Type,
                    ConfigurationIndex = element.ConfigurationIndex
                });
        }

        public void Apply(BackstitchesMarked @event)
        {
            foreach (var bs in (IEnumerable<BackstitchCoordinates>) @event.Backstitches)
                if (backstitches.TryGetValue((bs.X1, bs.Y1, bs.X2, bs.Y2), out var backstitch))
                    backstitch.Marked = true;
        }

        public void Apply(BackstitchesUnmarked @event)
        {
            foreach (var bs in (IEnumerable<BackstitchCoordinates>) @event.Backstitches)
                if (backstitches.TryGetValue((bs.X1, bs.Y1, bs.X2, bs.Y2), out var backstitch))
                    backstitch.Marked = false;
        }

        public void Apply(StitchesMarked @event)
        {
            foreach (var s in @event.Stitches)
                if (stitches.TryGetValue((s.X, s.Y), out var stitch))
                    stitch.Marked = true;
        }

        public void Apply(StitchesUnmarked @event)
        {
            foreach (var s in @event.Stitches)
                if (stitches.TryGetValue((s.X, s.Y), out var stitch))
                    stitch.Marked = false;
        }

        public Pattern GetPattern()
        {
            var result = new Pattern
            {
                Id = id,
                OwnerId = ownerId,
                Width = width,
                Height = height,
                Canvas = new Canvas {Title = canvas.Title},
                Info = new Info
                {
                    Title = info.Title,
                    Author = info.Author,
                    Company = info.Company,
                    Copyright = info.Copyright
                },
                Strands = new Strands
                {
                    Backstitch = strands.Backstitch,
                    FrenchKnot = strands.FrenchKnot,
                    Full = strands.Full,
                    Half = strands.Half,
                    Petit = strands.Petit,
                    Quarter = strands.Quarter,
                    ThreeQuarter = strands.ThreeQuarter
                }
            };

            foreach (var config in configuration)
                result.Configurations.Add(new StitchConfiguration
                {
                    Symbol = config.Symbol,
                    HexColor = config.HexColor,
                    Strands = config.Strands
                });

            foreach (var ((x, y), stitch) in stitches)
                result.Stitches.Add(new Stitch
                {
                    X = x,
                    Y = y,
                    Marked = stitch.Marked,
                    Type = stitch.Type,
                    ConfigurationIndex = stitch.ConfigurationIndex
                });

            foreach (var ((x1, y1, x2, y2), backstitch) in backstitches)
                result.Backstitches.Add(new Backstitch
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Marked = backstitch.Marked,
                    ConfigurationIndex = backstitch.ConfigurationIndex
                });

            foreach (var element in elements)
                result.Elements.Add(new Element
                {
                    X = element.X,
                    Y = element.Y,
                    Type = element.Type,
                    ConfigurationIndex = element.ConfigurationIndex
                });

            return result;
        }

        public PatternOwner GetPatternOwner() => new PatternOwner {OwnerId = ownerId};
        public StitchesMarked MarkStitches(IEnumerable<StitchCoordinates> items) => new StitchesMarked {SourceId = id, Stitches = {items}};
        public StitchesUnmarked UnmarkStitches(IEnumerable<StitchCoordinates> items) => new StitchesUnmarked {SourceId = id, Stitches = {items}};
        public BackstitchesMarked MarkBackstitches(IEnumerable<BackstitchCoordinates> items) => new BackstitchesMarked {SourceId = id, Backstitches = {items}};
        public BackstitchesUnmarked UnmarkBackstitches(IEnumerable<BackstitchCoordinates> items) => new BackstitchesUnmarked {SourceId = id, Backstitches = {items}};
        public PatternDeleted Delete() => new PatternDeleted {SourceId = id};
    }
}
