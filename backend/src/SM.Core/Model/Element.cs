namespace SM.Core.Model
{
    public class Element
    {
        public Point Point { get; set; }
        public int ConfigurationIndex { get; set; }
        public ElementType Type { get; set; }
    }

    public enum ElementType
    {
        Undefined,
        FrenchKnot,
        Bead
    }
}