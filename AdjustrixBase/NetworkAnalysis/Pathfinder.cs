
namespace AdjustrixBase.NetworkAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AdjustrixBase.DataModels;

    public class Pathfinder<TNode, TEdge>
        where TNode : INode
        where TEdge : IEdge<TNode>, new()
    {
        private readonly Dictionary<TNode, List<TEdge>> _graphData;
        private readonly List<TNode> traversedNodes = new();

        public Pathfinder()
        {
            _graphData = new();
        }

        public Pathfinder(List<TEdge> paths)
        {
            _graphData = new();
            PathsToEdge(paths);
        }

        public Pathfinder(Dictionary<TNode, List<TEdge>> graphData)
        {
            _graphData = graphData;
        }

        public void PathsToEdge(List<TEdge> edges)
        {
            foreach (TEdge edge in edges)
            {
                PathToEdge(edge);
            }
            MakeGraphTwoSided();
        }

        public void PathToEdge(TEdge edge)
        {
            InsertEdge(edge);
        }

        private void InsertEdge(TEdge edge)
        {
            if (_graphData.ContainsKey(edge.FromPoint))
            {
                _graphData[edge.FromPoint].Add(edge);
            }
            else
            {
                _graphData[edge.FromPoint] = new List<TEdge> { edge };
            }
        }

        private void MakeGraphTwoSided()
        {
            foreach (TNode key in _graphData.Keys.ToList())
            {
                foreach (TEdge edge in _graphData[key])
                {
                    if (!_graphData.ContainsKey(edge.ToPoint))
                    {
                        _graphData[edge.ToPoint] = new List<TEdge>();
                    }
                    //TEdge reverseEdge = new(key, edge.Length);
                    TEdge reverseEdge = new();
                    reverseEdge.FromPoint = edge.ToPoint;
                    reverseEdge.ToPoint = edge.FromPoint;
                    reverseEdge.Length = edge.Length;
                    if (!_graphData[edge.ToPoint].Contains(reverseEdge))
                    {
                        _graphData[edge.ToPoint].Add(reverseEdge);
                    }
                }
            }
        }

        public List<TNode> AStar(TNode startNode, TNode targetNode)
        {
            // Sorted array for keeping the paths with possible shortest distance at the top
            var heap = new SortedSet<Tuple<double, TNode>>();
            var distances = _graphData.Keys.ToDictionary(node => node, _ => double.MaxValue);
            distances[startNode] = 0;
            var previous = new Dictionary<TNode, TNode>();
            var completePaths = new List<Tuple<List<TNode>, double>>();

            heap.Add(new Tuple<double, TNode>(0, startNode));

            while (heap.Any())
            {
                var (currentDistance, currentNode) = heap.First();
                heap.Remove(heap.First());

                if (currentDistance > distances[currentNode])
                {
                    continue;
                }

                if (!(currentDistance < distances[targetNode] || !completePaths.Any()))
                {
                    continue;
                }

                if (currentNode.Number == targetNode.Number)
                {
                    var node = targetNode;
                    var path = new List<TNode>();
                    while (node.Number != startNode.Number)
                    {
                        path.Add(node);
                        node = previous[node];
                    }
                    path.Add(startNode);
                    path.Reverse();
                    completePaths.Add(new Tuple<List<TNode>, double>(path, distances[targetNode]));
                    continue;
                }

                foreach (TEdge edge in _graphData[currentNode])
                {

                    if (previous.ContainsKey(currentNode) && previous[currentNode].Number == edge.ToPoint.Number)
                    { // To prevent the algorithm to cover the same TEdge twice
                        continue;
                    }
                    if (traversedNodes.Contains(edge.ToPoint))
                    { // To make sure all TEdges are traversed regardless of whether they are too long
                        continue;
                    }

                    double tentativeDistance = distances[currentNode] + edge.Length;
                    if (tentativeDistance < distances[edge.ToPoint])
                    {
                        heap.Add(new Tuple<double, TNode>(tentativeDistance, edge.ToPoint));
                        distances[edge.ToPoint] = tentativeDistance;
                        previous[edge.ToPoint] = currentNode;
                    }
                }
            }

            try
            {
                List<TNode> shortestTraverse = completePaths.OrderBy(p => p.Item2).First().Item1;
                return shortestTraverse;
            }
            catch (Exception)
            {
                throw;
            }
            //return new ();
        }

        public double GetMinDistanceBetween(TNode point1, TNode point2)
        {
            double totalDistance = 0;
            List<TNode> closestPath = AStar(point1, point2);
            for (int i = 0; i < closestPath.Count - 1; i++)
            {
                TEdge currentEdge = _graphData[closestPath[i]].Where(x => x.ToPoint.Number == closestPath[i + 1].Number).First();
                totalDistance += currentEdge.Length;
            }
            return totalDistance;
        }

        public void ClearTraversedPoints()
        {
            traversedNodes.Clear();
        }
    }
}
