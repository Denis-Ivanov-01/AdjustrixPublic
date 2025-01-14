namespace Adjustment
{
    public class Benchmark : PointBase, IOneDimPoint, INode
    {
        public Benchmark(string number, double? x, double? y) : base(number, x, y)
        {

        }
    }

    public class KnownBenchmark : KnownPointNoCoordsBase, IOneDimPoint, INode
    {

        public KnownBenchmark(string number, double? x, double? y, double value) : base(number, x, y)
        {
            Value = value;
        }

        public KnownBenchmark(string number, double value) : base(number)
        {
            Value = value;
        }
    }

    public class NewBenchmark : PointBase, INode
    {
        public NewBenchmark(string number, double? x, double? y) : base(number, x, y)
        {

        }

        public NewBenchmark(string number) : base(number)
        {

        }
    }

    public class AdjustedPoint : PointBase, IOneDimPoint, INode
    { //TODO: figure out if this must be in the base file
        // maybe branch out one dimensional adjustments (gravimetric, nivelation)
        public double Value { get; set; }

        public AdjustedPoint(string number, double value, double? x, double? y) : base(number, x, y)
        {
            Value = value;
        }
    }
}
