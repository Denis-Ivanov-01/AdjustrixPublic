namespace AdjustrixBase.DataModels
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
        public PointBase(string n, double v)
        {
            Number = n;
            Value = v;
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

        public bool EqualsObj(PointBase? other)
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

        public override bool Equals(object? other)
        {
            if (other == null)
            {
                return false;
            }

            try
            {
                PointBase p = (PointBase)other;
                return p.Number == Number;
            }
            catch (Exception) { return false; }

            // Only the number should be compared.
            // The program should check for duplicate points by number and by coordinates
            // when the data is received.
            //return Number == other.Number /*&& X == other.X && Y == other.Y*/;
        }

        public override int GetHashCode()
        {
            return Number?.GetHashCode() ?? 0;
        }

        public static bool operator ==(PointBase? left, PointBase? right)
        {
            // Handle null cases
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            // Delegate to Equals
            return left.Equals(right);
        }

        // Overload the != operator
        public static bool operator !=(PointBase? left, PointBase? right)
        {
            return !(left == right);
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
