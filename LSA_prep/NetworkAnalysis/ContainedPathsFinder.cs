namespace LSA_Base
{
    /// <summary>
    /// A cluster of known poins with the following properties:
    /// 1) A cluster can have one or more points inside and the paths that they are a part of
    /// 2) The distance between two clusters is equal to the shortest distance between
    ///     a point from cluster A and a point from cluster B.
    /// </summary>
    internal class KnownPointsCluster
    {
        public HashSet<PointBase> _points;
        public List<List<PointBase>> _paths;

        public KnownPointsCluster()
        {
            _points = new();
            _paths = new();
        }

        public void AddPath(List<PointBase> path)
        {
            _paths.Add(path);
            int[] knownPIndices = GetKnownPointIndices(path);
            _points = GetKnownPointByIndices(path, knownPIndices).ToHashSet();
        }

        public static int[] GetKnownPointIndices(List<PointBase> path)
        {
            int knownPointsCount = path.Where(x => IsKnownPoint(x)).Count();
            int[] indices = new int[knownPointsCount];
            int currIndex = 0;
            for (int i = 0; i < path.Count; i++)
            {
                PointBase currP = path[i];
                if (IsKnownPoint(currP))
                {
                    indices[currIndex] = i;
                    currIndex++;
                }
            }
            return indices;
        }

        public static bool IsKnownPoint(PointBase p)
        {
            return p is KnownPointNoCoordsBase;
        }

        public static List<PointBase> GetKnownPointByIndices(List<PointBase> path, int[] indices)
        {
            List<PointBase> knownPoints = new();
            for (int i = 0; i < indices.Length; i++)
            {
                knownPoints.Add(path[indices[i]]);
            }
            return knownPoints;
        }
    }


    internal class ContainedPathsFinder<TEdge>
        where TEdge : IEdge<PointBase, TEdge>, new()
    {
        private readonly List<List<PointBase>> closedPaths;
        private readonly List<KnownPointsCluster> knownPointsClusters;
        private readonly Pathfinder<PointBase, TEdge> _pathfinder;

        public ContainedPathsFinder(List<List<PointBase>> closedP, Pathfinder<PointBase, TEdge> pathfinder)
        {
            closedPaths = closedP;
            knownPointsClusters = DefineClustersOnInit();
            _pathfinder = pathfinder;
        }

        public static bool IsContainedPath(List<PointBase> path)
        {
            return path[0] != path[^1] && path[0] is KnownPointNoCoordsBase && path[^1] is KnownPointNoCoordsBase;
        }

        public static bool HasKnownPoint(List<PointBase> path)
        {
            return path.Where(x => x is KnownPointNoCoordsBase).Any();
        }

        private Tuple<PointBase, PointBase, double> CalcDistBetweenClusters(KnownPointsCluster k1, KnownPointsCluster k2)
        {
            List<Tuple<PointBase, PointBase, double>> distances = new();
            foreach (PointBase k1Point in k1._points)
            {
                foreach (PointBase k2Point in k2._points)
                {
                    double dist = _pathfinder.GetMinDistanceBetween(k1Point, k2Point);
                    distances.Add(Tuple.Create(k1Point, k2Point, dist));
                }
            }
            return distances.OrderBy(x => x.Item3).First();
        }

        private List<KnownPointsCluster> DefineClustersOnInit()
        {
            List<List<PointBase>> pathsWithKnownPoint = closedPaths.Where(x => HasKnownPoint(x)).ToList();
            List<KnownPointsCluster> clusters = new();
            while (true)
            {
                if (pathsWithKnownPoint.Count == 0)
                {
                    break;
                }
                List<PointBase> currPath = pathsWithKnownPoint.First();
                (KnownPointsCluster currCluster, pathsWithKnownPoint) = DefineClusterFromPaths(currPath, pathsWithKnownPoint);
                clusters.Add(currCluster);
            }
            return clusters;
        }

        /// <summary>
        /// Looks for a pair of paths that connect two Known Points. If such path is found, it is added to the cluster.
        /// </summary>
        /// <param name="path">A path with a Known Point in it.</param>
        /// <param name="containedPaths">Contained paths with KnownPoints in them</param>
        /// <returns></returns>
        private static (KnownPointsCluster, List<List<PointBase>>) DefineClusterFromPaths(List<PointBase> path, List<List<PointBase>> containedPaths)
        { //todo: refactor this cuz I wanna hang myself when I look at it ... disgusting
            KnownPointsCluster cluster = new();
            cluster.AddPath(path);
            containedPaths.Remove(path);
            while (true)
            {
                bool connectedPointsFound = false;
                foreach (List<PointBase> currPath in containedPaths)
                {
                    int[] knownPIndicex = KnownPointsCluster.GetKnownPointIndices(currPath);
                    List<PointBase> knownPoints = KnownPointsCluster.GetKnownPointByIndices(currPath, knownPIndicex);
                    if (cluster._points.Any(x => knownPoints.Any(kp => kp.Number == x.Number)))
                    {
                        cluster.AddPath(currPath);
                        containedPaths.Remove(currPath);
                        connectedPointsFound = true;
                        break;
                    }
                }
                if (!connectedPointsFound)
                {
                    break;
                }
            }
            return (cluster, containedPaths);
        }

        /// <summary>
        /// Find the combinations of Known Points that must be connected in order to achieve linear independancy.
        /// </summary>
        /// <returns></returns>
        public List<Tuple<PointBase, PointBase>> FindKnownPointsToConnect()
        {
            // Generating a list of all gravimetric points that are to be connected
            List<Tuple<PointBase, PointBase>> containedPathsPoints = new();
            while (true)
            {
                if (knownPointsClusters.Count <= 1)
                {
                    break;
                }

                List<Tuple<KnownPointsCluster, KnownPointsCluster, PointBase, PointBase, double>> clustersCombos = new();
                for (int i = 0; i < knownPointsClusters.Count - 1; i++)
                {//Getting all cluster combinations to find the one closest to each other
                    for (int j = i + 1; j < knownPointsClusters.Count; j++)
                    {
                        KnownPointsCluster c1 = knownPointsClusters[i];
                        KnownPointsCluster c2 = knownPointsClusters[j];
                        (PointBase fromP, PointBase toP, double dist) = CalcDistBetweenClusters(c1, c2);
                        clustersCombos.Add(Tuple.Create(c1, c2, fromP, toP, dist));
                    }
                }
                clustersCombos = clustersCombos.OrderBy(item => item.Item5).ToList();
                Tuple<KnownPointsCluster, KnownPointsCluster, PointBase, PointBase, double> t = clustersCombos.First();
                PointBase p1 = t.Item3;
                PointBase p2 = t.Item4;
                containedPathsPoints.Add(Tuple.Create(p1, p2));
                CombineClusters(t.Item1, t.Item2);
            }
            return containedPathsPoints;
        }

        private void CombineClusters(KnownPointsCluster c1, KnownPointsCluster c2)
        {
            KnownPointsCluster resultCluster = new();
            resultCluster._points = c1._points.Union(c2._points).ToHashSet();
            resultCluster._paths.AddRange(c1._paths);
            resultCluster._paths.AddRange(c2._paths);
            knownPointsClusters.Add(resultCluster);
            knownPointsClusters.Remove(c1);
            knownPointsClusters.Remove(c2);
        }
    }
}
