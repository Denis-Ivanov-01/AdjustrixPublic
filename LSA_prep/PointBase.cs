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

    public abstract class PointBase: INode
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
    }

    public abstract class KnownPointNoCoordsBase : PointBase
    {
        public KnownPointNoCoordsBase(string number, double? x, double? y) : base(number, x, y)
        {

        }
    }
}
