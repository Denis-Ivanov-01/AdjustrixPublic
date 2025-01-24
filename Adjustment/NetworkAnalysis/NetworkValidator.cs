namespace Adjustment.NetworkAnalysis
{
    public class NetworkValidator<TEdge>
        where TEdge : IEdge<PointBase, TEdge>, IDirectedMeasurement, new()
    {
        private Dictionary<PointBase, List<TEdge>> graphData = new();

        /// <summary>
        /// Intended to be executed after the graph is made two-sided
        /// </summary>
        /// <param name="graphData"></param>
        public void PerformSecondaryValidation(Dictionary<PointBase, List<TEdge>> graphData)
        {
            //todo: test this and replace the error handling with custom exceptions
            this.graphData = graphData;
            AssertAtLeastTwoNeighbors();
            AssertGraphIsConnected();
            AssertNoCycles();
        }

        /// <summary>
        /// Intended to be executed on the raw measurements before any processing from the graph
        /// </summary>
        /// <param name="measurements"></param>
        public void PerformInitialValidation(List<TEdge> measurements)
        {
            //todo: debug this
            AssertNoDuplicates(measurements);
        }

        public void AssertAtLeastTwoNeighbors()
        {
            foreach (var point in graphData.Keys)
            {
                var neighbors = GetNeighbors(point);

                if (neighbors.Count < 2)
                {
                    throw new HangingPointException($"Point {point.Number} has fewer than 2 neighbors.", point.Number);
                }
            }
        }

        // There must be a path from each point to every other point
        public void AssertGraphIsConnected()
        {//Maybe this should be done for every point. Think about it.
            Pathfinder<PointBase, TEdge> pf = new(graphData);

            List<PointBase> points = graphData.Keys.ToList();

            for (int i = 0; i < points.Count - 1; i++)
            {
                PointBase p1 = points[i];
                for (int j = i + 1; j < points.Count; j++)
                {
                    PointBase p2 = points[j];
                    try
                    {
                        pf.AStar(p1, p2);
                    }
                    catch (Exception)
                    {
                        throw new NetworkNotConnectedException("The network is not fully connected!");
                    }
                }
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
                        throw new DuplicateMeasurementException("Placeholder exception to be replaced later.", point.Number, otherPoint.Number);
                    }
                }
            }
        }

        public void AssertNoDuplicates(List<TEdge> measurements)
        {
            for (int i = 0; i < measurements.Count - 1; i++)
            {
                TEdge meas1 = measurements[i];
                for (int j = i + 1; j < measurements.Count; j++)
                {
                    TEdge meas2 = measurements[j];
                    if (MeasurementsMirrored(meas1, meas2))
                    {
                        throw new LoopingMeasurementsException("Loop found in the measurements!", meas1.FromPoint.Number, meas1.ToPoint.Number);
                    }
                    if (MeasurementsDuplicate(meas1, meas2))
                    {
                        throw new DuplicateMeasurementException("Duplicate measurements found!", meas1.FromPoint.Number, meas1.ToPoint.Number);
                    }
                }
            }
        }

        private bool MeasurementsMirrored(TEdge meas1, TEdge meas2)
        {
            return meas1.FromPoint.Number == meas2.ToPoint.Number &&
                        meas1.ToPoint.Number == meas2.FromPoint.Number;
        }

        private bool MeasurementsDuplicate(TEdge meas1, TEdge meas2)
        {
            return meas1.FromPoint.Number == meas2.FromPoint.Number &&
                meas1.ToPoint.Number == meas2.ToPoint.Number;
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
