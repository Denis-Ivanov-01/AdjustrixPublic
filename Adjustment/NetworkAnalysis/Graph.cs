namespace Adjustment.NetworkAnalysis
{
    public class Graph<TEdge>
        where TEdge : IEdge<PointBase, TEdge>, IDirectedMeasurement, new()
    {
        public int measurementsCount = 0;
        public Dictionary<PointBase, List<TEdge>> graphData = new();
        public readonly List<TEdge> initialMeasurements;
        public readonly Pathfinder<PointBase, TEdge> pathfinder;

        public Graph(List<TEdge> measurements)
        {
            NetworkDataValidator<TEdge> validator = new();
            validator.PerformInitialValidation(measurements);
            MeasurementsToEdge(measurements);
            initialMeasurements = measurements;
            pathfinder = new(graphData);
            MakeGraphTwoSided();
            validator.PerformSecondaryValidation(graphData);
        }

        public HashSet<(PointBase, PointBase)> MeasurementsFromTraverse(List<PointBase> traverse)
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

        public void InsertEdge(TEdge edge)
        {
            if (!graphData.Keys.Any(x => x.Number == edge.FromPoint.Number))
            //if (!graphData.ContainsKey(edge.FromPoint))
            {
                graphData[edge.FromPoint] = new List<TEdge>();
            }
            if (!graphData[edge.FromPoint].Any(e => e.FromPoint.Number == edge.FromPoint.Number
            && e.ToPoint.Number == edge.ToPoint.Number))
            {
                graphData[edge.FromPoint].Add(edge);
            }
        }

        public void RemoveEdgeByPoint(PointBase starPointBase, PointBase endNode)
        {
            TEdge edge = graphData[starPointBase].Where(x => x.ToPoint.Number == endNode.Number).First();
            int index = graphData[starPointBase].IndexOf(edge);
            graphData[starPointBase].RemoveAt(index);
        }

        public void RemoveEdgeIfExists(PointBase starPointBase, PointBase endNode)
        {
            if (graphData[starPointBase].Where(x => x.ToPoint == endNode).Count() > 0)
            {
                RemoveEdgeByPoint(starPointBase, endNode);
            }
        }

        public void MakeGraphTwoSided()
        {
            foreach (PointBase key in graphData.Keys.ToList())
            {
                foreach (TEdge edge in graphData[key])
                {
                    //if (!graphData.Keys.Any(x => x.Number == edge.ToPoint.Number))
                    if (!graphData.ContainsKey(edge.ToPoint))
                    {
                        graphData[edge.ToPoint] = new List<TEdge>();
                    }
                    TEdge reverseEdge = edge.Reverse();
                    if (!graphData[edge.ToPoint].Any(e => e.FromPoint.Number == reverseEdge.FromPoint.Number
                    && e.ToPoint.Number == reverseEdge.ToPoint.Number))
                    {
                        graphData[edge.ToPoint].Add(reverseEdge);
                    }
                }
            }
        }

        public void RemoveMultipleEdges(List<TEdge> edges)
        {
            foreach (TEdge e in edges)
            {
                //RemoveEdgeIfExists(starPointBase, endNode);
                graphData[e.FromPoint].Remove(e);
            }
        }

        public void InsertMultipleEdges(List<TEdge> edges)
        {
            foreach (TEdge e in edges)
            {
                InsertEdge(e);
            }
        }

    }
}
