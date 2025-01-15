using System.Text.Json.Serialization;

namespace Adjustment
{
    public class HeightDelta : MeasurementBase<PointBase, HeightDelta>, IOneDimMeasurement
    {
        public double Value { get; set; }

        public override PointBase FromPoint { get; set; }

        public override PointBase ToPoint { get; set; }

        [JsonConstructor]
        public HeightDelta(PointBase FromPoint, PointBase ToPoint, double Value, double Length, bool IsReversed = false)
            : base(FromPoint, ToPoint, Length, IsReversed)
        {
            this.Value = Value;
            this.FromPoint = FromPoint;
            this.ToPoint = ToPoint;
        }

        public HeightDelta(PointBase FromPoint, PointBase ToPoint, double Value, bool Negative = false)
            : base(FromPoint, ToPoint, MathFunctions.CalcDistBetweenPoints(FromPoint, ToPoint), Negative)
        {
            this.Value = Value;
        }

        public HeightDelta() : base() { }

        public override HeightDelta Reverse()
        {
            return new HeightDelta(this.ToPoint, this.FromPoint, -this.Value, this.Length);
        }
    }
}
