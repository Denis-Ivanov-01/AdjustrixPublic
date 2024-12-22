using MathNet.Numerics.LinearAlgebra;

namespace LSA_Base
{
    public class GravimetricAdjustment : Adjustment<RelativeMeasurement, AdjustedPoint>
    {
        private const int decimalPrecision = 12;

        protected override int RequiredDecimalPrecision { get { return decimalPrecision; } }

        public GravimetricAdjustment(List<List<PointBase>> distinctTraverses, List<RelativeMeasurement> measurements)
            : base(distinctTraverses, measurements)
        {

        }

        public override RelativeMeasurement CreateNegativeMeasurement(RelativeMeasurement meas)
        {
            return new RelativeMeasurement(meas.FromPoint, meas.ToPoint, -meas.Value, meas.Length, true);
        }

        public override Matrix<double> CreateConfigurationMatrix()
        {
            Matrix<double> confMatrix = matrixBuilder.Dense(Measurements.Count, DistinctTraverses.Count);
            foreach (List<RelativeMeasurement> traverse in AssignedApproxMeasurements)
            {
                int travIndex = AssignedApproxMeasurements.IndexOf(traverse);
                foreach (RelativeMeasurement measurement in traverse)
                {
                    int measIndex = GetMeasurementIndex(measurement);
                    confMatrix[measIndex, travIndex] = GetMeasurementConfigurationIndex(measurement);
                }
            }
            return confMatrix;
        }

        public override Matrix<double> CreateWeightMatrix()
        {
            int matrixSize = Measurements.Count;
            Matrix<double> weightMatrix = matrixBuilder.Dense(matrixSize, matrixSize);
            foreach (RelativeMeasurement meas in Measurements)
            {
                int currIndex = GetMeasurementIndex(meas);
                weightMatrix[currIndex, currIndex] = meas.Length;
            }
            return weightMatrix;
        }

        public override Vector<double> CalculateInnacuraciesVector(List<List<RelativeMeasurement>> assignedMeasurements)
        {
            Vector<double> innacuracies = vectorBuilder.Dense(assignedMeasurements.Count);
            foreach (List<RelativeMeasurement> trav in assignedMeasurements)
            {
                int travIndex = assignedMeasurements.IndexOf(trav);
                innacuracies[travIndex] = CalculateInaccuracy(trav);
            }
            return innacuracies;
        }

        public override Vector<double> CalculateCorrections(Vector<double> pvMatrix, Matrix<double> weightMatrix)
        {
            Vector<double> corrections = vectorBuilder.Dense(Measurements.Count);
            foreach (RelativeMeasurement meas in Measurements)
            {
                int i = GetMeasurementIndex(meas);
                corrections[i] = weightMatrix[i, i] * pvMatrix[i];
            }
            return corrections;
        }

        public override List<AdjustedPoint> CalculateUnknownPoints(List<RelativeMeasurement> adjustedMeasurements)
        {
            NetworkAnalyzer<RelativeMeasurement> graph = new NetworkAnalyzer<RelativeMeasurement>();
            graph.MeasurementsToEdge(adjustedMeasurements.Cast<RelativeMeasurement>().ToList());
            List<NewGravimetricPoint> newPoints = Points
                .Where(p => p is not KnownGravimetricPoint).Cast<NewGravimetricPoint>().ToList();
            List<AdjustedPoint> adjustedPoints = new();
            KnownGravimetricPoint kp = (KnownGravimetricPoint)Points.Where(x => x is KnownGravimetricPoint).First();
            foreach (NewGravimetricPoint newPoint in newPoints)
            {
                List<PointBase> trav = graph.AStar(kp, newPoint);
                graph.ClearTraversePoints();
                List<RelativeMeasurement> measurements = AssignMeasurementsToTraverse(trav, adjustedMeasurements).Cast<RelativeMeasurement>().ToList();
                double value = kp.Value + measurements.Sum(meas => meas.Value);
                adjustedPoints.Add(new AdjustedPoint(newPoint.Number, value, newPoint.X, newPoint.Y));
            }
            return adjustedPoints;
        }

        public override List<RelativeMeasurement> CalculateAdjustedMeasurements(Vector<double> corrections)
        {
            List<RelativeMeasurement> adjustedMeasurements = new();
            foreach (RelativeMeasurement meas in Measurements)
            {
                int correspondingIndex = GetMeasurementIndex(meas);
                double adjustedValue = meas.Value + corrections[correspondingIndex];
                adjustedMeasurements.Add(new RelativeMeasurement(meas.FromPoint, meas.ToPoint, adjustedValue, meas.Length));
            }
            return adjustedMeasurements;
        }

        private double CalculateInaccuracy(List<RelativeMeasurement> trav)
        {
            if (IsLinkedTraverse(trav))
            {
                return CalculateLinkedTraverseInaccuracy(trav);
            }
            return CalculateClosedTraverseInaccuracy(trav);
        }

        private static double CalculateClosedTraverseInaccuracy(List<RelativeMeasurement> trav)
        {
            double currInaccuracy = 0;
            foreach (RelativeMeasurement measurement in trav)
            {
                currInaccuracy += measurement.Value;
            }
            return currInaccuracy;
        }

        private static int GetMeasurementConfigurationIndex(RelativeMeasurement meas)
        {
            if (meas.IsReversed)
            {
                return -1;
            }
            return 1;
        }

        private double CalculateLinkedTraverseInaccuracy(List<RelativeMeasurement> trav)
        {
            (PointBase startP, PointBase toP) = GetTravStartEndPoint(trav);
            KnownGravimetricPoint startPoint = (KnownGravimetricPoint)startP;
            KnownGravimetricPoint toPoint = (KnownGravimetricPoint)toP;
            double currInaccuracy = trav.Sum(meas => meas.Value);
            double betweenPointsValue = toPoint.Value - startPoint.Value;
            //Console.WriteLine($"Between points {betweenPointsValue}");
            double result = currInaccuracy - betweenPointsValue;
            return result;
        }
    }
}
