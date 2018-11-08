namespace SM.Service.Patterns.Xsd
{
    public class Node
    {
        public enum NodeType
        {
            Undefined = 0,
            FrenchKnot = 1,
            Bead = 2
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

        public ItemType ItemType
        {
            get
            {
                if (Type == NodeType.Bead)
                    return ItemType.Bead;
                return ItemType.FrenchKnot;
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
                Color.HasItems |= ItemType.Bead;
                return;
            }
            Color.HasItems |= ItemType.FrenchKnot;
        }
    }
}
