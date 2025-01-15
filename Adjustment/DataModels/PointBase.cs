namespace Adjustment
{

    public interface INode
    {
        public string Number { get; set; }
    }

    public interface IOneDimPoint
    {
        public double Value { get; set; }
    }

    public class PointBase : INode, IEquatable<PointBase>
    {//todo: use this as benchmark base (one dim point base)
        public string Number { get; set; }
        public double Value { get; set; }
        public double? X;
        public double? Y;

        public PointBase()
        {

        }

        //[JsonConstructor]
        public PointBase(string Number, double Value)
        {
            this.Number = Number;
            this.Value = Value;
        }

        public PointBase(string Number, double? X = null, double? Y = null)
        {
            this.Number = Number;
            this.X = X;
            this.Y = Y;
        }

        public PointBase(string Number)
        {
            this.Number = Number;
        }

        public bool Equals(PointBase? other)
        {
            if (other == null)
            {
                return false;
            }

            // Only the number should be compared.
            // The program should check for duplicate points by number and by coordinates
            // when the data is received.
            return Number == other.Number /*&& X == other.X && Y == other.Y*/;
        }
    }

    public abstract class KnownPointNoCoordsBase : PointBase
    {
        public KnownPointNoCoordsBase(string number, double? x, double? y) : base(number, x, y)
        {

        }

        public KnownPointNoCoordsBase(string number, double value) : base(number, value)
        {

        }

        public KnownPointNoCoordsBase(string number) : base(number)
        {

        }
    }
}
