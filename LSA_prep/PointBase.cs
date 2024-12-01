namespace LSA_Base
{

    public interface INode
    {
        public string Number { get; set; }
    }

    public interface IOneDimPoint
    {
        public double Value { get; set; }
    }

    public abstract class PointBase: INode, IEquatable<PointBase>
    {
        public string Number { get; set; }
        public double? X;
        public double? Y;

        public PointBase(string number, double? x=null, double? y = null)
        {
            Number = number;
            X = x;
            Y = y;
        }

        public bool Equals(PointBase? other)
        {
            if (other == null) return false;

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
    }
}
