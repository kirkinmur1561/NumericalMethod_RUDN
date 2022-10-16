namespace Lab_3
{
    public readonly struct PointD
    {
        public decimal X { get; init; }
        public decimal Y { get; init; }
        

        public PointD(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() =>
            $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";

    }
}