namespace Adjustment
{
    public class HeightDifference : MeasurementBase<PointBase, HeightDifference>, IOneDimMeasurement
    {
        public double Value { get; set; }

        public override PointBase FromPoint { get; set; }

        public override PointBase ToPoint { get; set; }

        public HeightDifference(PointBase fromPoint, PointBase toPoint, double heightDiff, double length, bool negative = false) : base(fromPoint, toPoint, length, negative)
        {
            Value = heightDiff;
        }

        public HeightDifference(PointBase fromPoint, PointBase toPoint, double heightDiff, bool negative = false) : base(fromPoint, toPoint, MathFunctions.CalcDistBetweenPoints(fromPoint, toPoint), negative)
        {
            Value = heightDiff;
        }

        public HeightDifference() : base() { }

        public override HeightDifference Reverse()
        {
            return new HeightDifference(this.ToPoint, this.FromPoint, this.Length);
        }
    }
}
