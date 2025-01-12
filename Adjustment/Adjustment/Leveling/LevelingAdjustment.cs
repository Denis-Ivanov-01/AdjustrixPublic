using MathNet.Numerics.LinearAlgebra;

namespace Adjustment
{
    public class LevelingAdjustment : Adjustment<HeightDifference, AdjustedPoint>
    {
        private const int decimalPrecision = 12;

        protected override int RequiredDecimalPrecision { get { return decimalPrecision; } }

        public LevelingAdjustment(List<List<PointBase>> distinctTraverses, List<HeightDifference> measurements)
            : base(distinctTraverses, measurements)
        {

        }

        protected override HeightDifference CreateNegativeMeasurement(HeightDifference meas)
        {
            return new HeightDifference(meas.FromPoint, meas.ToPoint, -meas.Value, meas.Length, true);
        }

        protected override Matrix<double> CreateConfigurationMatrix()
        {
            Matrix<double> confMatrix = matrixBuilder.Dense(Measurements.Count, DistinctTraverses.Count);
            foreach (List<HeightDifference> traverse in AssignedApproxMeasurements)
            {
                int travIndex = AssignedApproxMeasurements.IndexOf(traverse);
                foreach (HeightDifference measurement in traverse)
                {
                    int measIndex = GetMeasurementIndex(measurement);
                    confMatrix[measIndex, travIndex] = GetMeasurementConfigurationIndex(measurement);
                }
            }
            return confMatrix;
        }

        protected override Matrix<double> CreateReversedWeightMatrix()
        {
            int matrixSize = Measurements.Count;
            Matrix<double> weightMatrix = matrixBuilder.Dense(matrixSize, matrixSize);
            foreach (HeightDifference meas in Measurements)
            {
                int currIndex = GetMeasurementIndex(meas);
                weightMatrix[currIndex, currIndex] = meas.Length;
            }
            return weightMatrix;
        }

        protected override Vector<double> CalculateResidualsVector(List<List<HeightDifference>> assignedMeasurements)
        {
            Vector<double> innacuracies = vectorBuilder.Dense(assignedMeasurements.Count);
            foreach (List<HeightDifference> trav in assignedMeasurements)
            {
                int travIndex = assignedMeasurements.IndexOf(trav);
                innacuracies[travIndex] = CalculateResidual(trav);
            }
            return innacuracies;
        }

        protected override Vector<double> CalculateCorrections(Vector<double> pvMatrix, Matrix<double> weightMatrix)
        {
            Vector<double> corrections = vectorBuilder.Dense(Measurements.Count);
            foreach (HeightDifference meas in Measurements)
            {
                int i = GetMeasurementIndex(meas);
                corrections[i] = weightMatrix[i, i] * pvMatrix[i];
            }
            return corrections;
        }

        protected override List<AdjustedPoint> CalculateUnknownPoints(List<HeightDifference> adjustedMeasurements)
        {
            List<HeightDifference> meas = adjustedMeasurements.Cast<HeightDifference>().ToList();
            Pathfinder<PointBase, HeightDifference> pathfinder = new(meas);

            List<NewBenchmark> newPoints = Points
                .Where(p => p is not KnownBenchmark).Cast<NewBenchmark>().ToList();
            List<AdjustedPoint> adjustedPoints = new();
            KnownBenchmark kp = (KnownBenchmark)Points.Where(x => x is KnownBenchmark).First();
            foreach (NewBenchmark newPoint in newPoints)
            {
                List<PointBase> trav = pathfinder.AStar(kp, newPoint);
                pathfinder.ClearTraversedPoints();
                List<HeightDifference> measurements = AssignMeasurementsToTraverse(trav, adjustedMeasurements).Cast<HeightDifference>().ToList();
                double value = kp.Value + measurements.Sum(meas => meas.Value);
                adjustedPoints.Add(new AdjustedPoint(newPoint.Number, value, newPoint.X, newPoint.Y));
            }
            return adjustedPoints;
        }

        protected override List<HeightDifference> CalculateAdjustedMeasurements(Vector<double> corrections)
        {
            List<HeightDifference> adjustedMeasurements = new();
            foreach (HeightDifference meas in Measurements)
            {
                int correspondingIndex = GetMeasurementIndex(meas);
                double adjustedValue = meas.Value + corrections[correspondingIndex];
                adjustedMeasurements.Add(new HeightDifference(meas.FromPoint, meas.ToPoint, adjustedValue, meas.Length));
            }
            return adjustedMeasurements;
        }

        private double CalculateResidual(List<HeightDifference> trav)
        {
            if (IsLinkedTraverse(trav))
            {
                return CalculateLinkedTraverseResidual(trav);
            }
            return CalculateClosedTraverseResidual(trav);
        }

        private static double CalculateClosedTraverseResidual(List<HeightDifference> trav)
        {
            double currResidual = 0;
            foreach (HeightDifference measurement in trav)
            {
                currResidual += measurement.Value;
            }
            return currResidual;
        }

        private static int GetMeasurementConfigurationIndex(HeightDifference meas)
        {
            if (meas.IsReversed)
            {
                return -1;
            }
            return 1;
        }

        private double CalculateLinkedTraverseResidual(List<HeightDifference> trav)
        {
            (PointBase startP, PointBase toP) = GetTravStartEndPoint(trav);
            KnownBenchmark startPoint = (KnownBenchmark)startP;
            KnownBenchmark toPoint = (KnownBenchmark)toP;
            double currResidual = trav.Sum(meas => meas.Value);
            double betweenPointsValue = toPoint.Value - startPoint.Value;
            double result = currResidual - betweenPointsValue;
            return result;
        }
    }
}
