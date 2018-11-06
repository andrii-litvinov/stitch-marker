using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;

namespace SM.Service.Patterns
{
    public class PatternAggregate
    {
        // TODO: [AL] Rename to Pattern.

        private string id;
        private string ownerId;
        private uint width;
        private uint height;
        private Canvas canvas;
        private Info info;
        private Strands strands;
        private readonly List<StitchConfiguration> configuration = new List<StitchConfiguration>();
        private readonly List<Stitch> stitches = new List<Stitch>();
        private readonly List<Backstitch> backstitches = new List<Backstitch>();
        private readonly List<Element> elements = new List<Element>();

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
            {
                configuration.Add(new StitchConfiguration
                {
                    Symbol = config.Symbol,
                    HexColor = config.HexColor,
                    Strands = config.Strands
                });
            }

            foreach (var stitch in pattern.Stitches)
                stitches.Add(new Stitch
                {
                    X = stitch.X,
                    Y = stitch.Y,
                    Type = stitch.Type,
                    ConfigurationIndex = stitch.ConfigurationIndex
                });

            foreach (var backstitch in pattern.Backstitches)
                backstitches.Add(new Backstitch
                {
                    X1 = backstitch.X1,
                    X2 = backstitch.X2,
                    Y1 = backstitch.Y1,
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
            foreach (var backstitch in (IEnumerable<BackstitchCoordinates>) @event.Backstitches)
            {
                var patternBackstitch = backstitches.SingleOrDefault(item =>
                    item.X1 == backstitch.X1 &&
                    item.Y1 == backstitch.Y1 &&
                    item.X2 == backstitch.X2 &&
                    item.Y2 == backstitch.Y2);
                if (patternBackstitch != null) patternBackstitch.Marked = true;
            }
        }

        public void Apply(BackstitchesUnmarked @event)
        {
            foreach (var backstitch in (IEnumerable<BackstitchCoordinates>) @event.Backstitches)
            {
                var patternBackstitch = backstitches.SingleOrDefault(item =>
                    item.X1 == backstitch.X1 &&
                    item.Y1 == backstitch.Y1 &&
                    item.X2 == backstitch.X2 &&
                    item.Y2 == backstitch.Y2);
                if (patternBackstitch != null) patternBackstitch.Marked = false;
            }
        }

        public void Apply(StitchesMarked @event)
        {
            foreach (var stitch in (IEnumerable<StitchCoordinates>) @event.Stitches)
            {
                var patternStitch = stitches.FirstOrDefault(item => item.X == stitch.X && item.Y == stitch.Y);
                if (patternStitch != null) patternStitch.Marked = true;
            }
        }

        public void Apply(StitchesUnmarked @event)
        {
            foreach (var stitch in (IEnumerable<StitchCoordinates>) @event.Stitches)
            {
                var patternStitch = stitches.FirstOrDefault(item => item.X == stitch.X && item.Y == stitch.Y);
                if (patternStitch != null) patternStitch.Marked = false;
            }
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
            {
                result.Configurations.Add(new StitchConfiguration
                {
                    Symbol = config.Symbol,
                    HexColor = config.HexColor,
                    Strands = config.Strands
                });
            }

            foreach (var stitch in stitches)
                result.Stitches.Add(new Stitch
                {
                    X = stitch.X,
                    Y = stitch.Y,
                    Type = stitch.Type,
                    ConfigurationIndex = stitch.ConfigurationIndex
                });

            foreach (var backstitch in backstitches)
                result.Backstitches.Add(new Backstitch
                {
                    X1 = backstitch.X1,
                    X2 = backstitch.X2,
                    Y1 = backstitch.Y1,
                    Y2 = backstitch.Y2,
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

        public StitchesMarked MarkStitches(IList<StitchCoordinates> items) => new StitchesMarked {SourceId = id, Stitches = {items}};
        public StitchesUnmarked UnmarkStitches(IList<StitchCoordinates> items) => new StitchesUnmarked {SourceId = id, Stitches = {items}};
        public BackstitchesMarked MarkBackstitches(IList<BackstitchCoordinates> items) => new BackstitchesMarked {SourceId = id, Backstitches = {items}};
        public BackstitchesUnmarked UnmarkBackstitches(IList<BackstitchCoordinates> items) => new BackstitchesUnmarked {SourceId = id, Backstitches = {items}};
        public PatternDeleted Delete() => new PatternDeleted {SourceId = id};
    }
}
