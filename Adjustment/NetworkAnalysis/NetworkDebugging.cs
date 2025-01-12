
namespace LSA_Base.NetworkAnalysis
{
    [Obsolete]
    internal partial class NetworkAnalyzer
    {

        public static void PrintTraverses(List<List<PointBase>> traverses)
        {
            foreach (List<PointBase> t in traverses)
            {
                foreach (PointBase point in t)
                {
                    Console.Write(point.Number + " ");
                }
                Console.WriteLine();
            }
        }

        public static void PrintTraverse(List<PointBase> traverse)
        {
            foreach (PointBase point in traverse)
            {
                Console.Write(point.Number + " ");
            }
            Console.WriteLine();
        }
    }
}
