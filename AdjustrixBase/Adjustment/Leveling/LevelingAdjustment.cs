using AdjustrixBase.Adjustment.Base;
using AdjustrixBase.DataModels;
using AdjustrixBase.NetworkAnalysis;
using MathNet.Numerics.LinearAlgebra;


namespace AdjustrixBase.Adjustment.Leveling
{
    public class LevelingAdjustment : Adjustment<HeightDelta, AdjustedBenchmark>
    {
        private const int decimalPrecision = 12;

        protected override int RequiredDecimalPrecision { get { return decimalPrecision; } }

        public LevelingAdjustment(List<List<PointBase>> distinctTraverses, List<HeightDelta> measurements)
            : base(distinctTraverses, measurements)
        {

        }

        protected override HeightDelta CreateNegativeMeasurement(HeightDelta meas)
        {
            return new HeightDelta(meas.FromPoint, meas.ToPoint, -meas.Value, meas.Length, true);
        }

        protected override Matrix<double> CreateConfigurationMatrix()
        {
            Matrix<double> confMatrix = matrixBuilder.Dense(Measurements.Count, DistinctTraverses.Count);
            foreach (List<HeightDelta> traverse in AssignedApproxMeasurements)
            {
                int travIndex = AssignedApproxMeasurements.IndexOf(traverse);
                foreach (HeightDelta measurement in traverse)
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
            foreach (HeightDelta meas in Measurements)
            {
                int currIndex = GetMeasurementIndex(meas);
                weightMatrix[currIndex, currIndex] = meas.Length;
            }
            return weightMatrix;
        }

        protected override Vector<double> CalculateResidualsVector(List<List<HeightDelta>> assignedMeasurements)
        {
            Vector<double> innacuracies = vectorBuilder.Dense(assignedMeasurements.Count);
            foreach (List<HeightDelta> trav in assignedMeasurements)
            {
                int travIndex = assignedMeasurements.IndexOf(trav);
                innacuracies[travIndex] = CalculateResidual(trav);
            }
            return innacuracies;
        }

        protected override Vector<double> CalculateCorrections(Vector<double> pvMatrix, Matrix<double> weightMatrix)
        {
            Vector<double> corrections = vectorBuilder.Dense(Measurements.Count);
            foreach (HeightDelta meas in Measurements)
            {
                int i = GetMeasurementIndex(meas);
                corrections[i] = weightMatrix[i, i] * pvMatrix[i];
            }
            return corrections;
        }

        protected override List<AdjustedBenchmark> CalculateUnknownPoints(List<HeightDelta> adjustedMeasurements,
            Matrix<double> Qv,
            double perUnitVariance)
        {
            List<HeightDelta> meas = adjustedMeasurements.Cast<HeightDelta>().ToList();
            Pathfinder<PointBase, HeightDelta> pathfinder = new(meas);

            List<NewBenchmark> newPoints = Points
                .Where(p => p is not KnownBenchmark).Cast<NewBenchmark>().ToList();

            List<AdjustedBenchmark> adjustedPoints = new();

            List<KnownBenchmark> knownBenchmarks = Points.Where(x => x is KnownBenchmark).
                Cast<KnownBenchmark>().ToList();

            KnownBenchmark kp = (KnownBenchmark)Points.Where(x => x is KnownBenchmark).First();

            Matrix<double> fi = matrixBuilder.Dense(newPoints.Count, adjustedMeasurements.Count);

            foreach (NewBenchmark newBenchmark in newPoints)
            {
                int benchmarkIndex = newPoints.IndexOf(newBenchmark);

                double minLength = double.PositiveInfinity;
                double optimalKnownBenchmarkElevation = double.NegativeInfinity;
                List<HeightDelta> optimalMeasurements = new();
                List<PointBase> optimalTraverse = new();

                foreach (KnownBenchmark knownBenchmark in knownBenchmarks)
                {
                    List<PointBase> trav = pathfinder.AStar(kp, newBenchmark);
                    List<HeightDelta> measurements = AssignMeasurementsToTraverse(trav, adjustedMeasurements);
                    double length = measurements.Sum(m => m.Length);
                    if (length < minLength)
                    {
                        minLength = length;
                        optimalKnownBenchmarkElevation = knownBenchmark.Value;
                        optimalMeasurements = measurements;
                        optimalTraverse = trav;
                    }
                }

                double elevation = optimalKnownBenchmarkElevation + optimalMeasurements.Sum(m => m.Value);
                adjustedPoints.Add(new AdjustedBenchmark(newBenchmark.Number, elevation, newBenchmark.X, newBenchmark.Y));

                foreach (HeightDelta m in optimalMeasurements)
                {
                    int mIndex = GetMeasurementIndex(m);
                    int value = GetMeasurementConfigurationIndex(m);
                    fi[benchmarkIndex, mIndex] = value;
                }
            }

            Matrix<double> KH = (fi.Multiply(Qv).Multiply(fi.Transpose())) *
                (perUnitVariance * perUnitVariance);

            int counter = 0;
            foreach (AdjustedBenchmark adjustedBenchmark in adjustedPoints)
            {
                adjustedBenchmark.Variance = Math.Sqrt(KH[counter, counter]);
                counter++;
            }

            //foreach (NewBenchmark newPoint in newPoints)
            //{
            //    List<PointBase> trav = pathfinder.AStar(kp, newPoint);
            //    pathfinder.ClearTraversedPoints();
            //    List<HeightDelta> measurements = AssignMeasurementsToTraverse(trav, adjustedMeasurements).Cast<HeightDelta>().ToList();
            //    double value = kp.Value + measurements.Sum(meas => meas.Value);
            //    adjustedPoints.Add(new AdjustedBenchmark(newPoint.Number, value, newPoint.X, newPoint.Y));
            //}
            return adjustedPoints;
        }

        protected override List<HeightDelta> CalculateAdjustedMeasurements(Vector<double> corrections)
        {
            List<HeightDelta> adjustedMeasurements = new();
            foreach (HeightDelta meas in Measurements)
            {
                int correspondingIndex = GetMeasurementIndex(meas);
                double adjustedValue = meas.Value + corrections[correspondingIndex];
                adjustedMeasurements.Add(new HeightDelta(meas.FromPoint, meas.ToPoint, adjustedValue, meas.Length));
            }
            return adjustedMeasurements;
        }

        private double CalculateResidual(List<HeightDelta> trav)
        {
            if (IsLinkedTraverse(trav))
            {
                return CalculateLinkedTraverseResidual(trav);
            }
            return CalculateClosedTraverseResidual(trav);
        }

        private static double CalculateClosedTraverseResidual(List<HeightDelta> trav)
        {
            double currResidual = 0;
            foreach (HeightDelta measurement in trav)
            {
                currResidual += measurement.Value;
            }
            return currResidual;
        }

        private static int GetMeasurementConfigurationIndex(HeightDelta meas)
        {
            if (meas.IsReversed)
            {
                return -1;
            }
            return 1;
        }

        private double CalculateLinkedTraverseResidual(List<HeightDelta> trav)
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
