namespace LSA_Base
{
    public class RelativeMeasurement : MeasurementBase<PointBase, RelativeMeasurement>, IOneDimMeasurement
    {
        public double Value { get; set; }

        public override PointBase FromPoint { get; set; }
        public override PointBase ToPoint { get; set; }

        public RelativeMeasurement(PointBase fromPoint, PointBase toPoint, double potentialDelta, double length, bool negative = false) : base(fromPoint, toPoint, length, negative)
        {
            Value = potentialDelta;
        }

        public RelativeMeasurement(PointBase fromPoint, PointBase toPoint, double potentialDelta, bool negative = false) : base(fromPoint, toPoint, MathFunctions.CalcDistBetweenPoints(fromPoint, toPoint), negative)
        {
            Value = potentialDelta;
        }

        public RelativeMeasurement() : base() { }

        public override RelativeMeasurement Reverse()
        {
            return new RelativeMeasurement(this.ToPoint, this.FromPoint, this.Length);
        }
    }
}
