namespace LSA_Base
{
    class PointListComparer<TPoint> : IEqualityComparer<List<TPoint>>
        where TPoint : PointBase
    {
        public bool Equals(List<TPoint>? x, List<TPoint>? y)
        {
            if (x == null || y == null) return x == y;

            if (x.Count != y.Count) return false;

            return x.SequenceEqual(y);
        }

        public int GetHashCode(List<TPoint> obj)
        {
            if (obj == null) return 0;

            unchecked
            {
                int hash = 19;
                foreach (var item in obj)
                {
                    hash = hash * 31 + (item == null ? 0 : item.GetHashCode());
                }
                return hash;
            }
        }
    }

}
