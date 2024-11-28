namespace LSA_Base
{
    public class GravimetricPoint : PointBase, IOneDimPoint, INode
    {
        public GravimetricPoint(string number, double? x, double? y) : base(number, x, y)
        {

        }

        public double Value { get; set; }
    }

    public class KnownGravimetricPoint : KnownPointNoCoordsBase
    {
        public double GravitationalPotential { get; set; }

        public KnownGravimetricPoint(string number, double? x, double? y , double value) : base(number, x, y)
        {
            GravitationalPotential = value;
        }
    }

    public class NewGravimetricPoint : PointBase
    {
        public NewGravimetricPoint(string number, double? x, double? y) : base(number, x, y)
        {
            
        }
    }

    public class AdjustedGravimetricPoint : PointBase
    {
        public readonly double GravitationalPotential;

        public AdjustedGravimetricPoint(string number, double potential, double? x, double? y) : base(number, x, y)
        {
            GravitationalPotential = potential;
        }
    }
}
