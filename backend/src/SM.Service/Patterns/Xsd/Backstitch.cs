namespace SM.Service.Patterns.Xsd
{
    public class Backstitch
    {
        private Color color;

        public ushort X1 { get; set; }
        public ushort X2 { get; set; }
        public ushort Y1 { get; set; }
        public ushort Y2 { get; set; }

        public Color Color
        {
            get => color;
            set
            {
                color = value;
                Color.HasItems |= Pattern.ItemType.BackStitch;
            }
        }
    }
}