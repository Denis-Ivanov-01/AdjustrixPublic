namespace LSA_Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    //internal struct Edge: IEdge<PointBase>
    //{
    //    public PointBase FromPoint { get; set; }

    //    public PointBase ToPoint { get; set; }

    //    public double Length { get; set; }


    //    public Edge(PointBase neighborPoint, double distance)
    //    {
    //        ToPoint = neighborPoint;
    //        Length = distance;
    //    }
    //}


    public class NetworkAnalyzer<TEdge>
        where TEdge : IEdge<PointBase, TEdge>, IDirectedMeasurement, new()
    {
        private int measurementsCount = 0;
        private readonly Dictionary<PointBase, List<TEdge>> graphData = new();
        private readonly List<PointBase> traversedPoints = new();
        private readonly List<List<PointBase>> linkedTraverseFromBreakdown = new();
        public static List<List<List<PointBase>>> traversesFromBreakDown = new();
        private readonly List<TEdge> initialMeasurements;

        public NetworkAnalyzer()
        {
            initialMeasurements = new();
        }

        public NetworkAnalyzer(List<TEdge> measurements)
        {
            MeasurementsToEdge(measurements);
            initialMeasurements = measurements;
        }

        #region Public methods
        //Entry point the the library
        public List<List<PointBase>> FindAllDistinctTraverses()
        {
            MakeGraphTwoSided();
            List<List<PointBase>> distinctTravs = new();
            List<List<PointBase>> closedTravs = FindAllClosedTraverses();
            //Console.WriteLine("closed travs...");
            //PrintTraverses(closedTravs);
            //Console.WriteLine("graph data before linked travs");
            //PrintGraphData();
            List<List<PointBase>> linkedTravs = FindAllLinkedTraverses(closedTravs);
            //Console.WriteLine("linked travs...");
            //PrintTraverses(linkedTravs);
            //List<HashSet<PointBase>> unorderedPaths = new();
            distinctTravs.AddRange(linkedTravs);
            distinctTravs.AddRange(closedTravs);
            distinctTravs = SupersetRemover.RemoveSupersetsTwoTypes(distinctTravs);
            distinctTravs = SupersetRemover.RemoveCompositeSupersets(distinctTravs, linkedTraverseFromBreakdown);
            //Console.WriteLine("Distinct Paths before removing");
            //PrintTraverses(distinctTravs);
            distinctTravs = EnsureCorrectResult(distinctTravs);
            //Console.WriteLine("Distinct Paths after removing");
            //PrintTraverses(distinctTravs);
            ValidateResult(distinctTravs);
            return distinctTravs;
        }

        private int CalculateRedundancy()
        {
            int knownPointsCount = graphData.Keys.OfType<KnownPointNoCoordsBase>().Count();
            int unknownPointsCount = graphData.Keys.Count - knownPointsCount;
            return measurementsCount - unknownPointsCount;
        }

        private void ValidateResult(List<List<PointBase>> distinctTraverses)
        {//todo: move one outside this class. Should be done in the class that calls this one!
            int redundancy = CalculateRedundancy();
            int distinctTravsCount = distinctTraverses.Count;
            if (redundancy != distinctTravsCount)
            {//todo: in that case, the user should be informed of the problem in the geometry

                throw new IncorrectGeometryAnalysis("The number of distinct traverses is not equal to the redundancy!");
            }
        }

        /// <summary>
        /// If there are more distinct traverses than the redundancy, it removes the unnecessary ones. 
        /// </summary>
        private List<List<PointBase>> EnsureCorrectResult(List<List<PointBase>> distinctTraverses)
        {
            int redundancy = CalculateRedundancy();
            int distinctTravsCount = distinctTraverses.Count;
            if (redundancy > distinctTravsCount)
            {
                throw new IncorrectGeometryAnalysis("Not enough distinct traverses were found! " +
                    "Contact the developer");
            }
            if (redundancy == distinctTravsCount) { return distinctTraverses; }
            while (distinctTravsCount > redundancy)
            {
                distinctTraverses = RemoveRedundantTraverse(distinctTraverses);
                distinctTravsCount--;
            }
            return distinctTraverses;
        }

        private List<List<PointBase>> RemoveRedundantTraverse(List<List<PointBase>> distinctTraverses)
        { // todo: This should be able to remove more than one traverse at a time
            // The idea is to remove N traverses, so that we reach the needed amount (redundancy = traverses)
            // Then we check if all measurments are included
            // or maybe this works as well :/
            distinctTraverses = distinctTraverses.OrderByDescending(x => x.Count).ToList();
            for (int i = 0; i < distinctTraverses.Count; i++)
            {
                List<PointBase> currTrav = distinctTraverses[i];
                distinctTraverses.RemoveAt(i);
                if (IncludeAllMeasurements(distinctTraverses)) { return distinctTraverses; }
                distinctTraverses.Insert(i, currTrav);
            }
            // should be unreachable, but who knows :/
            throw new IncorrectGeometryAnalysis("No traverse can be removed :/ your network is fucked");
        }

        private bool IncludeAllMeasurements(List<List<PointBase>> distinctTraverses)
        {
            HashSet<(PointBase, PointBase)> measurementsInTraverses = new();
            foreach (List<PointBase> trav in distinctTraverses)
            {
                measurementsInTraverses = measurementsInTraverses.Union(MeasurementsFromTraverse(trav)).ToHashSet();
            }
            HashSet<(PointBase, PointBase)> measurements = new();
            foreach (TEdge meas in initialMeasurements)
            {
                measurements.Add((meas.FromPoint, meas.ToPoint));
            }

            return measurements.All(x => measurementsInTraverses.Contains(x));
        }

        private static HashSet<(PointBase, PointBase)> MeasurementsFromTraverse(List<PointBase> traverse)
        {
            HashSet<(PointBase, PointBase)> measurementsInTraverse = new();
            for (int i = 0; i < traverse.Count - 1; i++)
            {
                measurementsInTraverse.Add((traverse[i], traverse[i + 1]));
                measurementsInTraverse.Add((traverse[i + 1], traverse[i]));
            }
            return measurementsInTraverse;
        }

        public void MeasurementsToEdge(List<TEdge> measurements)
        {
            foreach (TEdge measurement in measurements)
            {
                MeasurementToEdge(measurement);
            }
            MakeGraphTwoSided();
        }

        public void MeasurementToEdge(TEdge measurement)
        { //todo: make private or even remove after testing is complete
            measurementsCount += 1;
            InsertEdge(measurement);
        }
        #endregion

        #region Common

        public List<PointBase> AStar(PointBase starPointBase, PointBase targePointBase)
        { // todo: move to a separate class?

            //todo: instead of calculating the distances here using coords, they should be pre-calculated
            // There should be an option to just pass the distances for each traverse as input data!

            // Sorted array for keeping the paths with possible shortest distance at the top
            var heap = new SortedSet<Tuple<double, PointBase>>();
            var distances = graphData.Keys.ToDictionary(node => node, _ => double.MaxValue);
            distances[starPointBase] = 0;
            var previous = new Dictionary<PointBase, PointBase>();
            var completePaths = new List<Tuple<List<PointBase>, double>>();

            heap.Add(new Tuple<double, PointBase>(0, starPointBase));

            while (heap.Any())
            {
                var (currentDistance, currenPointBase) = heap.First();
                heap.Remove(heap.First());

                if (currentDistance > distances[currenPointBase])
                {
                    continue;
                }

                if (!(currentDistance < distances[targePointBase] || !completePaths.Any()))
                {
                    continue;
                }

                if (currenPointBase.Number == targePointBase.Number)
                {
                    var node = targePointBase;
                    var path = new List<PointBase>();
                    while (node.Number != starPointBase.Number)
                    {
                        path.Add(node);
                        node = previous[node];
                    }
                    path.Add(starPointBase);
                    path.Reverse();
                    completePaths.Add(new Tuple<List<PointBase>, double>(path, distances[targePointBase]));
                    continue;
                }

                foreach (TEdge neighbor in graphData[currenPointBase])
                {

                    if (previous.ContainsKey(currenPointBase) && previous[currenPointBase].Number == neighbor.ToPoint.Number)
                    { // To prevent the algorithm to cover the same edge twice
                        continue;
                    }
                    if (traversedPoints.Contains(neighbor.ToPoint))
                    { // To make sure all edges are traversed regardless of whether they are too long
                        continue;
                    }

                    double tentativeDistance = distances[currenPointBase] + neighbor.Length;
                    if (tentativeDistance < distances[neighbor.ToPoint])
                    {
                        //Console.WriteLine($"Adding {tentativeDistance} distance and {neighbor.GetType()} {neighbor.Number}");
                        heap.Add(new Tuple<double, PointBase>(tentativeDistance, neighbor.ToPoint));
                        distances[neighbor.ToPoint] = tentativeDistance;
                        previous[neighbor.ToPoint] = currenPointBase;
                    }
                }
            }

            try
            {
                List<PointBase> shortestTraverse = completePaths.OrderBy(p => p.Item2).First().Item1;
                return shortestTraverse;
            }
            catch (Exception)
            {
                throw;
            }
            //return new ();
        }

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

        public void ClearTraversePoints()
        {
            traversedPoints.Clear();
        }

        private void InsertEdge(TEdge edge)
        {
            //TEdge edge = new(endNode, MathFunctions.CalcDistBetweenPoints(starPointBase, endNode));
            //TEdge edge = new();
            //edge.FromPoint = startNode;
            //edge.ToPoint = endNode;
            if (!graphData.ContainsKey(edge.FromPoint))
            {
                graphData[edge.FromPoint] = new List<TEdge>();
            }
            if (!graphData[edge.FromPoint].Any(e => e.FromPoint == edge.FromPoint && e.ToPoint == edge.ToPoint))
            {
                graphData[edge.FromPoint].Add(edge);
            }
            //if (graphData.ContainsKey(edge.FromPoint))
            //{
            //    graphData[edge.FromPoint].Add(edge);
            //}
            //else
            //{
            //    graphData[edge.FromPoint] = new List<TEdge> { edge };
            //}
        }

        private void RemoveEdgeByPoint(PointBase starPointBase, PointBase endNode)
        {
            TEdge edge = graphData[starPointBase].Where(x => x.ToPoint == endNode).First();
            int index = graphData[starPointBase].IndexOf(edge);
            graphData[starPointBase].RemoveAt(index);
        }

        private void RemoveEdgeIfExists(PointBase starPointBase, PointBase endNode)
        {
            if (graphData[starPointBase].Where(x => x.ToPoint == endNode).Count() > 0/*Contains(endNode)*/)
            {
                RemoveEdgeByPoint(starPointBase, endNode);
            }
        }

        private void MakeGraphTwoSided()
        {
            foreach (PointBase key in graphData.Keys.ToList())
            {
                foreach (TEdge edge in graphData[key])
                {
                    if (!graphData.ContainsKey(edge.ToPoint))
                    {
                        graphData[edge.ToPoint] = new List<TEdge>();
                    }
                    //TEdge reverseEdge = new(key, edge.Length);
                    TEdge reverseEdge = edge.Reverse();
                    reverseEdge.FromPoint = edge.ToPoint;
                    reverseEdge.ToPoint = edge.FromPoint;
                    reverseEdge.Length = edge.Length;
                    //reverseEdge.IsReversed = true;
                    if (!graphData[edge.ToPoint].Any(e => e.FromPoint == reverseEdge.FromPoint && e.ToPoint == reverseEdge.ToPoint))
                    //if (!graphData[edge.ToPoint].Contains(reverseEdge))
                    {
                        graphData[edge.ToPoint].Add(reverseEdge);
                    }
                }
            }
        }

        private List<PointBase> BreadthFirstSearch(PointBase start)
        {
            List<PointBase> visited = new();
            var queue = new Queue<PointBase>();

            visited.Add(start);
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (graphData.ContainsKey(current))
                {
                    foreach (var edge in graphData[current])
                    {
                        if (!visited.Contains(edge.ToPoint))
                        {
                            visited.Add(edge.ToPoint);
                            queue.Enqueue(edge.ToPoint);
                        }
                    }
                }
            }
            return visited;
        }

        private static List<List<PointBase>> GetAllPointCombinations(List<TEdge> inputList)
        {
            var result = new List<List<PointBase>>();
            GetAllCombinationsRecursive(inputList, 0, new List<PointBase>(), result);
            result.Add(new List<PointBase>());
            return result;
        }

        private static void GetAllCombinationsRecursive(List<TEdge> inputList, int index, List<PointBase> current, List<List<PointBase>> result)
        {
            // Skip adding the initial empty set by ensuring current is not empty
            if (current.Count > 0)
            {
                result.Add(new List<PointBase>(current));
            }

            // Iterate over the remaining elements
            for (int i = index; i < inputList.Count; i++)
            {
                // Include the current element in the combination
                current.Add(inputList[i].ToPoint);

                // Recurse to explore further combinations
                GetAllCombinationsRecursive(inputList, i + 1, current, result);

                // Exclude the last added element and continue
                current.RemoveAt(current.Count - 1);
            }
        }

        private void RemoveMultipleEdges(PointBase starPointBase, List<PointBase> endNodes)
        {
            foreach (PointBase endNode in endNodes)
            {
                RemoveEdgeIfExists(starPointBase, endNode);
            }
        }

        private void InsertMultipleEdges(PointBase starPoint, List<PointBase> endNodes)
        {
            foreach (PointBase endNode in endNodes)
            {
                //todo: replace this with a IEdgeFactory?
                TEdge edge = new();
                edge.FromPoint = starPoint;
                edge.ToPoint = endNode;
                edge.Length = MathFunctions.CalcDistBetweenPoints(starPoint, endNode);
                InsertEdge(edge);
            }
        }
        #endregion

        #region Determining simplest closed traverses

        private List<List<PointBase>> FindAllClosedTraverses()
        {

            HashSet<List<PointBase>> hsallClosedTravs = new();
            PointBase firstNode = graphData.Keys.OrderBy(x => graphData[x].Count).First();
            List<PointBase> pointsToWalk = BreadthFirstSearch(firstNode);

            foreach (var node in pointsToWalk)
            {
                if (!HasAvailableNeighbors(node))
                {
                    traversedPoints.Add(node);
                    continue;
                }
                List<List<PointBase>> closedPathsToNode = FindShortestPathToSelf(node);
                hsallClosedTravs.UnionWith(closedPathsToNode);
                traversedPoints.Add(node);
            }
            List<List<PointBase>> allClosedTravs = hsallClosedTravs.Distinct(new PointListComparer<PointBase>()).OrderByDescending(x => x.Count).ToList();
            PrintGraphData();
            Console.WriteLine("all closed travs");
            PrintTraverses(allClosedTravs);
            allClosedTravs = SupersetRemover.RemoveSupersetsOneType(allClosedTravs);
            Console.WriteLine("after one type remove");
            PrintTraverses(allClosedTravs);

            //this was added so that complex closed traverses would be eliminated.
            //otherwise, the algorithm finds huge closed traverses that encapsulate multiple smaller ones.
            //it should work fine, but test it using multiple configurations!
            allClosedTravs = SupersetRemover.RemoveCompositeSupersets(allClosedTravs, new List<List<PointBase>>());
            Console.WriteLine("after composite rmeove");
            PrintTraverses(allClosedTravs);

            allClosedTravs = BreakClosedTraversesToContained(allClosedTravs);
            Console.WriteLine("after breaking");
            PrintTraverses(allClosedTravs);

            allClosedTravs = SupersetRemover.RemoveSupersetsTwoTypes(allClosedTravs);
            Console.WriteLine("two type remove");
            PrintTraverses(allClosedTravs);

            allClosedTravs = SupersetRemover.RemoveSupersetsOneType(allClosedTravs);
            Console.WriteLine("one type remove");
            PrintTraverses(allClosedTravs);

            traversedPoints.Clear();
            return allClosedTravs;
        }

        private bool HasAvailableNeighbors(PointBase node)
        {
            return graphData[node].Where(p => !traversedPoints.Contains(p.ToPoint)).Count() > 1;
        }

        private List<List<PointBase>> FindShortestPathToSelf(PointBase node)
        { //todo: Refactor this piece of shit!
            HashSet<List<PointBase>> paths = new();
            foreach (var edge in graphData[node].OrderBy(x => x.Length))
            {
                //todo: make sure this shit is correct. What is the purpose of this?
                if (!HasTraversableNeighbor(node, edge) || traversedPoints.Contains(edge.ToPoint))
                {
                    continue;
                }
                //todo: remove after ensuring this is obsolete
                //if (/*graphData[neighbor].Count == 0 || */traversedPoints.Contains(neighbor))
                //{
                //    continue;
                //}
                //todo: replace this with a RemoveEdgeByPoints?? Think about a solution
                RemoveEdgeIfExists(edge.ToPoint, node);
                List<List<PointBase>> neighborCombinations = GetAllPointCombinations(graphData[edge.ToPoint]);
                try
                {
                    HashSet<List<PointBase>> currPaths = FindPossiblePaths(node, edge, neighborCombinations);
                    paths.UnionWith(currPaths);
                }
                finally
                {
                    //InsertEdge(edge);

                    // adding the reverse because we were iterating the neighbors and removing the edge
                    // from the neighbor to the node, not from the node to the neighbor
                    InsertEdge(edge.Reverse());
                }
            }
            return paths.OrderBy(p => p.Count).ToList();
        }

        private HashSet<List<PointBase>> FindPossiblePaths(PointBase node, TEdge edge, List<List<PointBase>> neighborCombinations)
        {
            HashSet<List<PointBase>> currPaths = new();
            foreach (List<PointBase> possibleCombination in neighborCombinations)
            {
                RemoveMultipleEdges(edge.ToPoint, possibleCombination);
                try
                { //todo: maybe at the end try to remove this try-catch-finally shit
                    List<PointBase> res = AStar(edge.ToPoint, node);
                    res.Insert(0, res[^1]);
                    if (res.Distinct().Count() <= 2)
                    {
                        continue;
                    }
                    currPaths.Add(res);
                }
                catch (Exception)
                {

                }
                finally
                {
                    InsertMultipleEdges(edge.ToPoint, possibleCombination);
                }
            }

            return currPaths;
        }

        private bool HasTraversableNeighbor(PointBase node, TEdge edge)
        {
            return graphData[edge.ToPoint].Where(e => !traversedPoints.Contains(e.ToPoint) && e.ToPoint != node).Any();
        }

        private static List<PointBase> GetClosedTraverseSection(List<PointBase> traverse, int startIndex, int endIndex)
        {
            if (startIndex == endIndex) { throw new InvalidDataException("Cannot get section with the same start and end index!"); }
            List<PointBase> section = new();
            if (startIndex < endIndex)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    section.Add(traverse[i]);
                }
            }
            else
            {
                for (int i = startIndex; i < traverse.Count - 1; i++)
                {
                    section.Add(traverse[i]);
                }
                for (int i = 0; i <= endIndex; i++)
                {
                    section.Add(traverse[i]);
                }
            }
            return section;
        }

        private static List<int> GetPointsIndices(List<PointBase> points, List<PointBase> traverse)
        {
            List<int> indices = new();
            foreach (PointBase point in points)
            {
                for (int i = 0; i < traverse.Count; i++)
                {
                    PointBase pointInPath = traverse[i];
                    if (indices.Contains(i)) { continue; }
                    if (point == pointInPath)
                    {
                        indices.Add(i);
                        break;
                    }
                }
            }
            return indices;
        }

        #endregion

        #region Determining simplest contained polygons
        private List<List<PointBase>> FindAllLinkedTraverses(List<List<PointBase>> closedTraverses)
        {
            List<List<PointBase>> containedTraverses = new();
            ContainedPathsFinder finder = new(closedTraverses);
            List<Tuple<PointBase, PointBase>> containedPathsPoints = finder.FindKnownPointsToConnect();
            foreach (Tuple<PointBase, PointBase> gpTuple in containedPathsPoints)
            {
                containedTraverses.Add(AStar(gpTuple.Item1, gpTuple.Item2));
            }
            Console.WriteLine("Contained paths end!");
            return containedTraverses;
            #region ProbablyObsolete
            //List<PointBase> knownPoints = graphData.Keys.Where(node => node.GetType() == typeof(KnownPointBase)).ToList();
            //if (knownPoints.Count < 2)
            //{
            //    return containedPaths;
            //}
            //List<Tuple<PointBase, PointBase>> knownPointsCombinations = new();
            //for (int firstPointIndex = 0; firstPointIndex < knownPoints.Count - 1; firstPointIndex++)
            //{//Iterating through all known points in the outer loop
            //    for (int secondPointIndex = firstPointIndex  + 1; secondPointIndex < knownPoints.Count; secondPointIndex++)
            //    {// In the inner loop we iterate through all the known points starting at the current outer known point
            //        // so that we don't get repeated combinations
            //        Tuple<PointBase, PointBase> currTuple = Tuple.Create(knownPoints[firstPointIndex], knownPoints[secondPointIndex]);
            //        knownPointsCombinations.Add(currTuple);
            //    }
            //}
            //foreach(Tuple<PointBase, PointBase> knownPointsTuple in knownPointsCombinations)
            //{
            //    containedPaths.Add(AStar(knownPointsTuple.Item1, knownPointsTuple.Item2));
            //}
            //SupersetRemover sr = new();
            //containedPaths = sr.RemoveSupersetsOneType(containedPaths);
            #endregion
        }

        private List<List<PointBase>> BreakClosedTraversesToContained(List<List<PointBase>> traverses)
        {
            List<List<PointBase>> brokenDownTravs = new();
            foreach (List<PointBase> t in traverses)
            {
                // Getting all the Points except the last one, because it is the same as the first one
                //List<PointBase> pathWithoutLastElement = path.GetRange(0, path.Count - 1);
                List<List<PointBase>> brokenDownTrav = BreakClosedTravToContainedTravs(t);
                if (brokenDownTrav.Count > 1)
                {
                    traversesFromBreakDown.Add(brokenDownTrav);
                }

                //PrintPaths(new List<List<PointBase>> { path });
                brokenDownTravs.AddRange(brokenDownTrav);
            }
            return brokenDownTravs;
        }

        private List<List<PointBase>> BreakClosedTravToContainedTravs(List<PointBase> traverse)
        {//todo: find the mistake in the logic -> 3453 is not returned, but lost

            List<List<PointBase>> result = new();
            //List<PointBase> knownPoints = path.Where(p => p.GetType() is KnownPointNoCoordsBase)).ToList();
            List<KnownPointNoCoordsBase> knownPoints = traverse.OfType<KnownPointNoCoordsBase>().ToList();
            int knownPointsCount = knownPoints.ToHashSet().Count;
            if (knownPointsCount < 2)
            {
                result.Add(traverse);
                return result;
            }
            List<int> knownPointIndices = GetPointsIndices(knownPoints.Cast<PointBase>().ToList(), traverse);
            List<Tuple<int, int>> indicesCombos = new();
            for (int i = 0; i < knownPointIndices.Count; i++)
            {
                if (i == knownPointIndices.Count - 1)
                {
                    indicesCombos.Add(Tuple.Create(knownPointIndices[i], knownPointIndices[0]));
                    continue;
                }
                indicesCombos.Add(Tuple.Create(knownPointIndices[i], knownPointIndices[i + 1]));
            }
            foreach (Tuple<int, int> indices in indicesCombos)
            {
                int startIndex = indices.Item1;
                int endIndex = indices.Item2;
                if (traverse[startIndex] == traverse[endIndex])
                {
                    continue;
                }
                List<PointBase> section = GetClosedTraverseSection(traverse, startIndex, endIndex);
                if (section.OfType<KnownPointNoCoordsBase>().Count() <= 2)
                {
                    if (section.Count > 2)
                    {
                        linkedTraverseFromBreakdown.Add(section);
                    }

                    result.Add(section);
                }
            }
            return result;
        }
        #endregion

        private void PrintGraphData()
        {
            foreach (KeyValuePair<PointBase, List<TEdge>> kvp in graphData)
            {
                PointBase key = kvp.Key;
                List<TEdge> edges = kvp.Value;
                Console.Write($"key {key.Number}: ");
                foreach (TEdge edge in edges)
                {
                    Console.Write($"{edge.ToPoint.Number} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }

    //internal class Pathfinder<TNode, TEdge>
    //    where TNode : INode
    //    where TEdge : IEdge<TNode, TEdge>, new()
    //{
    //    private readonly Dictionary<TNode, List<TEdge>> graphData = new();
    //    private readonly List<TNode> traversedNodes = new();

    //    public Pathfinder()
    //    {

    //    }

    //    public void MeasurementsToEdge(List<TEdge> edges)
    //    {
    //        foreach (TEdge edge in edges)
    //        {
    //            MeasurementToEdge(edge);
    //        }
    //        MakeGraphTwoSided();
    //    }

    //    public void MeasurementToEdge(TEdge edge)
    //    { //todo: make private or even remove after testing is complete
    //        InsertEdge(edge);
    //    }

    //    private void InsertEdge(TEdge edge)
    //    {
    //        if (graphData.ContainsKey(edge.FromPoint))
    //        {
    //            graphData[edge.FromPoint].Add(edge);
    //        }
    //        else
    //        {
    //            graphData[edge.FromPoint] = new List<TEdge> { edge };
    //        }
    //    }

    //    private void MakeGraphTwoSided()
    //    {
    //        foreach (TNode key in graphData.Keys.ToList())
    //        {
    //            foreach (TEdge edge in graphData[key])
    //            {
    //                if (!graphData.ContainsKey(edge.ToPoint))
    //                {
    //                    graphData[edge.ToPoint] = new List<TEdge>();
    //                }
    //                //TEdge reverseEdge = new(key, edge.Length);
    //                TEdge reverseEdge = new();
    //                reverseEdge.FromPoint = edge.ToPoint;
    //                reverseEdge.ToPoint = edge.FromPoint;
    //                reverseEdge.Length = edge.Length;
    //                if (!graphData[edge.ToPoint].Contains(reverseEdge))
    //                {
    //                    graphData[edge.ToPoint].Add(reverseEdge);
    //                }
    //            }
    //        }
    //    }

    //    public List<TNode> AStar(TNode startNode, TNode targetNode)
    //    { // todo: move to a separate class?

    //        //todo: instead of calculating the distances here using coords, they should be pre-calculated
    //        // There should be an option to just pass the distances for each traverse as input data!

    //        // Sorted array for keeping the paths with possible shortest distance at the top
    //        var heap = new SortedSet<Tuple<double, TNode>>();
    //        var distances = graphData.Keys.ToDictionary(node => node, _ => double.MaxValue);
    //        distances[startNode] = 0;
    //        var previous = new Dictionary<TNode, TNode>();
    //        var completePaths = new List<Tuple<List<TNode>, double>>();

    //        heap.Add(new Tuple<double, TNode>(0, startNode));

    //        while (heap.Any())
    //        {
    //            var (currentDistance, currentNode) = heap.First();
    //            heap.Remove(heap.First());

    //            if (currentDistance > distances[currentNode])
    //            {
    //                continue;
    //            }

    //            if (!(currentDistance < distances[targetNode] || !completePaths.Any()))
    //            {
    //                continue;
    //            }

    //            if (currentNode.Number == targetNode.Number)
    //            {
    //                var node = targetNode;
    //                var path = new List<TNode>();
    //                while (node.Number != startNode.Number)
    //                {
    //                    path.Add(node);
    //                    node = previous[node];
    //                }
    //                path.Add(startNode);
    //                path.Reverse();
    //                completePaths.Add(new Tuple<List<TNode>, double>(path, distances[targetNode]));
    //                continue;
    //            }

    //            foreach (TEdge edge in graphData[currentNode])
    //            {

    //                if (previous.ContainsKey(currentNode) && previous[currentNode].Number == edge.ToPoint.Number)
    //                { // To prevent the algorithm to cover the same TEdge twice
    //                    continue;
    //                }
    //                if (traversedNodes.Contains(edge.ToPoint))
    //                { // To make sure all TEdges are traversed regardless of whether they are too long
    //                    continue;
    //                }

    //                double tentativeDistance = distances[currentNode] + edge.Length;
    //                if (tentativeDistance < distances[edge.ToPoint])
    //                {
    //                    //Console.WriteLine($"Adding {tentativeDistance} distance and {neighbor.GetType()} {neighbor.Number}");
    //                    heap.Add(new Tuple<double, TNode>(tentativeDistance, edge.ToPoint));
    //                    distances[edge.ToPoint] = tentativeDistance;
    //                    previous[edge.ToPoint] = currentNode;
    //                }
    //            }
    //        }

    //        try
    //        {
    //            List<TNode> shortestTraverse = completePaths.OrderBy(p => p.Item2).First().Item1;
    //            return shortestTraverse;
    //        }
    //        catch (Exception)
    //        {
    //            throw;
    //        }
    //        //return new ();
    //    }
    //}
}
