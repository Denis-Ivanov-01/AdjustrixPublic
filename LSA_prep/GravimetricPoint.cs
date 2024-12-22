namespace LSA_Base
{
    public class GravimetricPoint : PointBase, IOneDimPoint, INode
    {
        public GravimetricPoint(string number, double? x, double? y) : base(number, x, y)
        {

        }

        public double Value { get; set; }
    }

    public class KnownGravimetricPoint : KnownPointNoCoordsBase, IOneDimPoint, INode
    {
        public double Value { get; set; }

        public KnownGravimetricPoint(string number, double? x, double? y, double value) : base(number, x, y)
        {
            Value = value;
        }
    }

    public class NewGravimetricPoint : PointBase, INode
    {
        public NewGravimetricPoint(string number, double? x, double? y) : base(number, x, y)
        {

        }
    }

    public class AdjustedPoint : GravimetricPoint
    { //TODO: figure out if this must be in the base file
        // maybe branch out one dimensional adjustments (gravimetric, nivelation)
        public double Value { get; set; }

        public AdjustedPoint(string number, double value, double? x, double? y) : base(number, x, y)
        {
            Value = value;
        }
    }
}
