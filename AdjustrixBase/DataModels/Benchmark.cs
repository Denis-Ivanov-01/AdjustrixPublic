using System.Text.Json.Serialization;

namespace AdjustrixBase.DataModels
{
    public class Benchmark : PointBase, IOneDimPoint, IEquatable<Benchmark>
    {
        public Benchmark(string number, double? x, double? y) : base(number, x, y)
        {

        }

        [JsonConstructor]
        public Benchmark(string Number, double Value) : base(Number, Value)
        {

        }

        public Benchmark(string number) : base(number) { }

        public bool Equals(Benchmark? other)
        {
            if (other == null)
            {
                return false;
            }
            return other.Number == Number;
        }

        public bool Equals(KnownBenchmark? other)
        {
            if (other == null)
            {
                return false;
            }
            return other.Number == Number;
        }

        public bool Equals(NewBenchmark? other)
        {
            if (other == null)
            {
                return false;
            }

            return other.Number == Number;
        }
    }

    public class KnownBenchmark : KnownPointNoCoordsBase, IOneDimPoint
    {

        public KnownBenchmark(string number, double? x, double? y, double value) : base(number, x, y)
        {
            Value = value;
        }

        [JsonConstructor]
        public KnownBenchmark(string Number, double Value) : base(Number)
        {
            this.Value = Value;
        }
    }

    public class NewBenchmark : Benchmark
    {
        public NewBenchmark(string number, double? x, double? y) : base(number, x, y)
        {

        }

        public NewBenchmark(string number) : base(number)
        {

        }
    }

    public class AdjustedBenchmark : PointBase, IOneDimPoint
    { //TODO: figure out if this must be in the base file
        // maybe branch out one dimensional adjustments (gravimetric, nivelation)
        public double Value { get; set; }

        public double Variance { get; set; }

        public AdjustedBenchmark(string number, double value, double? x, double? y) : base(number, x, y)
        {
            Value = value;
        }
    }
}
