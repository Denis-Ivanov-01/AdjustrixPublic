namespace Adjustment
{
    public class MathFunctions
    {
        public static double CalcDistBetweenPoints(PointBase p1, PointBase p2)
        {
            if (p1.X == null || p2.X == null || p1.Y == null || p2.Y == null)
            {
                throw new ArgumentNullException("A point with null coordinates was passed!");
            }
            double dx = (double)p2.X - (double)p1.X;
            double dy = (double)p2.Y - (double)p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
