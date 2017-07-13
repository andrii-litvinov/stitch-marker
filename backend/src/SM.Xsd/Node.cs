namespace SM.Xsd
{
    public class Node
    {
        public enum NodeType
        {
            Undefined,
            FrenchKnot,
            Bead
        }

        private Color color;

        private NodeType type;

        public ushort X;

        public ushort Y;

        public Color Color
        {
            get => color;
            set
            {
                color = value;
                SetColorItems();
            }
        }

        public Pattern.ItemType ItemType
        {
            get
            {
                if (Type == NodeType.Bead)
                    return Pattern.ItemType.Bead;
                return Pattern.ItemType.FrenchKnot;
            }
        }

        public NodeType Type
        {
            get => type;
            set
            {
                type = value;
                SetColorItems();
            }
        }

        private void SetColorItems()
        {
            if (Color == null || Type == NodeType.Undefined)
                return;
            if (Type == NodeType.Bead)
            {
                Color.HasItems |= Pattern.ItemType.Bead;
                return;
            }
            Color.HasItems |= Pattern.ItemType.FrenchKnot;
        }
    }
}