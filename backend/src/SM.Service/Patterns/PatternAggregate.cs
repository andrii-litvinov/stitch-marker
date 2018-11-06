using System.Collections.Generic;

namespace SM.Service.Patterns
{
    public class PatternAggregate
    {
        // TODO: [AL] Rename to Pattern.
        private readonly Dictionary<(uint, uint, uint, uint), Backstitch> backstitches = new Dictionary<(uint, uint, uint, uint), Backstitch>();
        private readonly List<StitchConfiguration> configuration = new List<StitchConfiguration>();
        private readonly List<Element> elements = new List<Element>();
        private readonly Dictionary<(uint, uint), Stitch> stitches = new Dictionary<(uint, uint), Stitch>();
        private Canvas canvas;
        private string id;
        private Info info;
        private string ownerId;
        private Strands strands;
        private uint width;
        private uint height;

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
            foreach (var eventBackstitch in (IEnumerable<BackstitchCoordinates>) @event.Backstitches)
            {
                backstitches.TryGetValue((eventBackstitch.X1, eventBackstitch.Y1, eventBackstitch.X2, eventBackstitch.Y2), out var backstitch);
                if (backstitch != null) backstitch.Marked = true;
            }
        }

        public void Apply(BackstitchesUnmarked @event)
        {
            foreach (var eventBackstitch in (IEnumerable<BackstitchCoordinates>) @event.Backstitches)
            {
                backstitches.TryGetValue((eventBackstitch.X1, eventBackstitch.Y1, eventBackstitch.X2, eventBackstitch.Y2), out var backstitch);
                if (backstitch != null) backstitch.Marked = false;
            }
        }

        public void Apply(StitchesMarked @event)
        {
            foreach (var eventStitch in (IEnumerable<StitchCoordinates>) @event.Stitches)
            {
                stitches.TryGetValue((eventStitch.X, eventStitch.Y), out var stitch);
                if (stitch != null) stitch.Marked = true;
            }
        }

        public void Apply(StitchesUnmarked @event)
        {
            foreach (var eventStitch in (IEnumerable<StitchCoordinates>) @event.Stitches)
            {
                stitches.TryGetValue((eventStitch.X, eventStitch.Y), out var stitch);
                if (stitch != null) stitch.Marked = false;
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
                result.Configurations.Add(new StitchConfiguration
                {
                    Symbol = config.Symbol,
                    HexColor = config.HexColor,
                    Strands = config.Strands
                });

            foreach (var stitch in stitches)
                result.Stitches.Add(new Stitch
                {
                    X = stitch.Value.X,
                    Y = stitch.Value.Y,
                    Marked = stitch.Value.Marked,
                    Type = stitch.Value.Type,
                    ConfigurationIndex = stitch.Value.ConfigurationIndex
                });

            foreach (var backstitch in backstitches)
                result.Backstitches.Add(new Backstitch
                {
                    X1 = backstitch.Value.X1,
                    X2 = backstitch.Value.X2,
                    Y1 = backstitch.Value.Y1,
                    Y2 = backstitch.Value.Y2,
                    Marked = backstitch.Value.Marked,
                    ConfigurationIndex = backstitch.Value.ConfigurationIndex
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
