namespace SM.Core.Model
{
    public struct Point
    {
        public long X { get; set; }
        public long Y { get; set; }

        public override string ToString() => $"X={X}, Y={Y}";
    }
}
