namespace Adjustment
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NetworkAnalysis;


    public partial class NetworkAnalyzer<TEdge>
        where TEdge : IEdge<PointBase, TEdge>, IDirectedMeasurement, new()
    {
        private int measurementsCount = 0;
        private readonly Dictionary<PointBase, List<TEdge>> graphData = new();
        private readonly List<PointBase> traversedPoints = new();
        private readonly List<List<PointBase>> linkedTraverseFromBreakdown = new();
        private readonly List<List<List<PointBase>>> traversesFromBreakdown = new();
        private readonly List<TEdge> initialMeasurements;
        private readonly Pathfinder<PointBase, TEdge> pathfinder;

        //public NetworkAnalyzer()
        //{
        //    initialMeasurements = new();
        //}

        public NetworkAnalyzer(List<TEdge> measurements)
        {
            NetworkValidator<TEdge> validator = new();
            validator.PerformInitialValidation(measurements);
            MeasurementsToEdge(measurements);
            initialMeasurements = measurements;
            pathfinder = new(graphData);
            MakeGraphTwoSided();
            validator.PerformSecondaryValidation(graphData);
        }

        //Entry point the library
        public List<List<PointBase>> FindAllDistinctTraverses()
        {
            List<List<PointBase>> distinctTravs = new();
            List<List<PointBase>> closedTravs = FindAllClosedTraverses();
            List<List<PointBase>> linkedTravs = FindAllLinkedTraverses(closedTravs);
            distinctTravs.AddRange(linkedTravs);
            distinctTravs.AddRange(closedTravs);
            distinctTravs = SupersetRemover.RemoveSupersetsTwoTypes(distinctTravs);
            distinctTravs = SupersetRemover.RemoveCompositeSupersets(distinctTravs, linkedTraverseFromBreakdown);
            distinctTravs = EnsureCorrectResult(distinctTravs);
            return distinctTravs;
        }

        #region Common
        private int CalculateRedundancy()
        {
            int knownPointsCount = graphData.Keys.OfType<KnownPointNoCoordsBase>().Count();
            int unknownPointsCount = graphData.Keys.Count - knownPointsCount;
            return measurementsCount - unknownPointsCount;
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
                    "Check for loops in the network!" +
                    "If no loops are found, contact the developer");
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
        {
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
            allClosedTravs = SupersetRemover.RemoveSupersetsOneType(allClosedTravs, traversesFromBreakdown);

            //this was added so that complex closed traverses would be eliminated.
            //otherwise, the algorithm can find huge closed traverses that encapsulate multiple smaller ones.
            //it should work fine, but test it using multiple configurations!
            allClosedTravs = SupersetRemover.RemoveCompositeSupersets(allClosedTravs, new List<List<PointBase>>());
            allClosedTravs = BreakClosedTraversesToLinked(allClosedTravs);
            allClosedTravs = SupersetRemover.RemoveSupersetsTwoTypes(allClosedTravs);
            allClosedTravs = SupersetRemover.RemoveSupersetsOneType(allClosedTravs, traversesFromBreakdown);
            traversedPoints.Clear();
            return allClosedTravs;
        }

        private bool HasAvailableNeighbors(PointBase node)
        {
            return graphData[node].Where(p => !traversedPoints.Contains(p.ToPoint)).Count() > 1;
        }

        private List<List<PointBase>> FindShortestPathToSelf(PointBase node)
        {
            HashSet<List<PointBase>> paths = new();
            foreach (var edge in graphData[node].OrderBy(x => x.Length))
            {

                if (!HasTraversableNeighbor(node, edge) || traversedPoints.Contains(edge.ToPoint))
                {
                    continue;
                }
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
                List<TEdge> edgesToRemove = graphData[edge.ToPoint].Where(e => possibleCombination.Contains(e.ToPoint)).ToList();
                RemoveMultipleEdges(edgesToRemove);
                try
                { //todo: maybe at the end try to remove this try-catch-finally shit
                    List<PointBase> res = pathfinder.AStar(edge.ToPoint, node);
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
                    InsertMultipleEdges(edgesToRemove);
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

        #region Determining simplest linked polygons
        private List<List<PointBase>> FindAllLinkedTraverses(List<List<PointBase>> closedTraverses)
        {
            List<List<PointBase>> linkedTraverses = new();
            LinkedTraverseFinder<TEdge> finder = new(closedTraverses, pathfinder);
            List<Tuple<PointBase, PointBase>> linkedTraversesPoints = finder.FindKnownPointsToConnect();
            foreach (Tuple<PointBase, PointBase> gpTuple in linkedTraversesPoints)
            {
                linkedTraverses.Add(pathfinder.AStar(gpTuple.Item1, gpTuple.Item2));
            }
            return linkedTraverses;
        }

        private List<List<PointBase>> BreakClosedTraversesToLinked(List<List<PointBase>> traverses)
        {
            List<List<PointBase>> brokenDownTravs = new();
            foreach (List<PointBase> t in traverses)
            {
                // Getting all the Points except the last one, because it is the same as the first one
                List<List<PointBase>> brokenDownTrav = BreakClosedTravToLinkedTravs(t);
                if (brokenDownTrav.Count > 1)
                {
                    traversesFromBreakdown.Add(brokenDownTrav);
                }

                brokenDownTravs.AddRange(brokenDownTrav);
            }
            return brokenDownTravs;
        }

        private List<List<PointBase>> BreakClosedTravToLinkedTravs(List<PointBase> traverse)
        {

            List<List<PointBase>> result = new();
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

        #region Utils
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
        {
            measurementsCount += 1;
            InsertEdge(measurement);
        }

        private void InsertEdge(TEdge edge)
        {
            if (!graphData.ContainsKey(edge.FromPoint))
            {
                graphData[edge.FromPoint] = new List<TEdge>();
            }
            if (!graphData[edge.FromPoint].Any(e => e.FromPoint == edge.FromPoint && e.ToPoint == edge.ToPoint))
            {
                graphData[edge.FromPoint].Add(edge);
            }
        }

        private void RemoveEdgeByPoint(PointBase starPointBase, PointBase endNode)
        {
            TEdge edge = graphData[starPointBase].Where(x => x.ToPoint == endNode).First();
            int index = graphData[starPointBase].IndexOf(edge);
            graphData[starPointBase].RemoveAt(index);
        }

        private void RemoveEdgeIfExists(PointBase starPointBase, PointBase endNode)
        {
            if (graphData[starPointBase].Where(x => x.ToPoint == endNode).Count() > 0)
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
                    TEdge reverseEdge = edge.Reverse();
                    //reverseEdge.FromPoint = edge.ToPoint;
                    //reverseEdge.ToPoint = edge.FromPoint;
                    //reverseEdge.Length = edge.Length;
                    if (!graphData[edge.ToPoint].Any(e => e.FromPoint == reverseEdge.FromPoint && e.ToPoint == reverseEdge.ToPoint))
                    {
                        graphData[edge.ToPoint].Add(reverseEdge);
                    }
                }
            }
        }

        private void RemoveMultipleEdges(List<TEdge> edges)
        {
            foreach (TEdge e in edges)
            {
                //RemoveEdgeIfExists(starPointBase, endNode);
                graphData[e.FromPoint].Remove(e);
            }
        }

        private void InsertMultipleEdges(List<TEdge> edges)
        {
            foreach (TEdge e in edges)
            {
                InsertEdge(e);
            }
        }
        #endregion
    }
}
