namespace LSA_Base
{
    public static class ListExtensions
    {
        public static void RemoveAtIndices<T>(this List<T> list, IEnumerable<int> indices)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list), "The list cannot be null!"); }
            if (indices == null)
            {
                throw new ArgumentNullException(nameof(indices), "The indices list cannot be null.");
            }

            indices = indices
            .Distinct() // Remove duplicates
            .Where(index => index >= 0 && index < list.Count) // Filter invalid indices
            .OrderByDescending(index => index);

            foreach (int i in indices)
            {
                list.RemoveAt(i);
            }
        }
    }
}
