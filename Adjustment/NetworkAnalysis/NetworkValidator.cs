namespace Adjustment.NetworkAnalysis
{
    public class NetworkValidator<TEdge>
        where TEdge : IEdge<PointBase, TEdge>, IDirectedMeasurement, new()
    {
        private readonly Dictionary<PointBase, List<TEdge>> graphData = new();

        public NetworkValidator(Dictionary<PointBase, List<TEdge>> graphData)
        {
            this.graphData = graphData;
            ValidateNetwork();
        }

        public void ValidateNetwork()
        {
            //todo: test this and replace the error handling with custom exceptions
            AssertAtLeastTwoNeighbors();
            AssertGraphIsConnected();
            AssertNoCycles();
        }

        public void AssertAtLeastTwoNeighbors()
        {
            foreach (var point in graphData.Keys)
            {
                var neighbors = GetNeighbors(point);

                if (neighbors.Count < 2)
                {
                    throw new InvalidOperationException($"Point {point.Number} has fewer than 2 neighbors.");
                }
            }
        }

        // There must be a path from each point to every other point
        public void AssertGraphIsConnected()
        {//Maybe this should be done for every point. Think about it.
            var visited = new HashSet<PointBase>();

            void DFS(PointBase point)
            {
                if (visited.Contains(point))
                {
                    return;
                }

                visited.Add(point);

                foreach (var neighbor in GetNeighbors(point))
                {
                    DFS(neighbor);
                }
            }

            // Start DFS from any point
            DFS(graphData.Keys.First());

            if (visited.Count != graphData.Keys.Count)
            {
                throw new InvalidOperationException("The graph is not fully connected.");
            }
        }

        // There must be only one path between two neighboring points (no cycles)
        public void AssertNoCycles()
        {//todo: perform this before making the graph two sided!
            foreach (PointBase point in graphData.Keys)
            {
                foreach (PointBase otherPoint in graphData.Keys.Where(x => x != point))
                {
                    if (graphData[point].Where(x => x.ToPoint == otherPoint).Count() >= 2)
                    {
                        throw new Exception("Placeholder exception to be replaced later.");
                    }
                }
            }
        }

        private List<PointBase> GetNeighbors(PointBase point)
        {
            if (!graphData.ContainsKey(point))
            {
                return new List<PointBase>();
            }

            return graphData[point]
                .Select(edge => edge.FromPoint == point ? edge.ToPoint : edge.FromPoint)
                .ToList();
        }
    }
}
