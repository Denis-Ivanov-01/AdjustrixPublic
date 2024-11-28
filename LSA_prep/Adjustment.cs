using MathNet.Numerics.LinearAlgebra;

namespace LSA_Base
{
    public abstract class Adjustment<TMeasurement, TAdjustedPoint>
        where TMeasurement : IEdge<PointBase, TMeasurement>, IDirectedMeasurement
        where TAdjustedPoint : PointBase, INode
    {//todo: make all public methods except AdjustNetwork protected after testing!
        public readonly MatrixBuilder<double> matrixBuilder = Matrix<double>.Build;
        public readonly VectorBuilder<double> vectorBuilder = Vector<double>.Build;
        public readonly HashSet<PointBase> Points;
        public readonly List<List<PointBase>> DistinctTraverses;
        public List<TMeasurement> Measurements;
        public readonly List<List<TMeasurement>> AssignedApproxMeasurements = new();
        private readonly Dictionary<int, TMeasurement> MeasurementIndices = new();
        protected abstract int RequiredDecimalPrecision { get; }

        public Adjustment(List<List<PointBase>> distinctTraverses, List<TMeasurement> measurements)
        {
            DistinctTraverses = distinctTraverses;
            Measurements = measurements;
            AsignMeasurementsIndices();
            AssignedApproxMeasurements = AsignMeasurementsToTraverses(measurements);
            Points = GetPoinsSet();
        }

        public AdjustmentResult<TMeasurement, TAdjustedPoint> AdjustNetwork()
        {
            
            Matrix<double> configurationMatrix = CreateConfigurationMatrix();
            Matrix<double> weightMatrix = CreateWeightMatrix();
            Matrix<double> normalMatrix = CalculateNormalMatrix(configurationMatrix, weightMatrix);
            Vector<double> initialInaccuracies = CalculateInnacuraciesVector(AssignedApproxMeasurements);
            Vector<double> kVector = CalculateK(normalMatrix, initialInaccuracies);
            Vector<double> pvVector = CalculatePV(kVector, configurationMatrix);
            Vector<double> corrections = CalculateCorrections(pvVector, weightMatrix);
            List<TMeasurement> adjustedMeasurements = CalculateAdjustedMeasurements(corrections);
            Vector<double> adjustedInaccuracies = CalculateInnacuraciesVector(AsignMeasurementsToTraverses(adjustedMeasurements));
            ValidateAdjustmentResult(adjustedInaccuracies);
            List<TAdjustedPoint> adjustedPoints = CalculateUnknownPoints(adjustedMeasurements);
            AdjustmentResult<TMeasurement, TAdjustedPoint> result = new(adjustedPoints, adjustedMeasurements, corrections);
            return result;
        }

        public abstract TMeasurement CreateNegativeMeasurement(TMeasurement meas);

        public abstract Matrix<double> CreateConfigurationMatrix();

        public abstract Matrix<double> CreateWeightMatrix();

        public abstract Vector<double> CalculateInnacuraciesVector(List<List<TMeasurement>> assignedMeasurements);

        public abstract Vector<double> CalculateCorrections(Vector<double> pvVector, Matrix<double> weightMatrix);

        public abstract List<TMeasurement> CalculateAdjustedMeasurements(Vector<double> corrections);

        public abstract List<TAdjustedPoint> CalculateUnknownPoints(List<TMeasurement> adjustedMeasurements);

        public Matrix<double> CalculateNormalMatrix(Matrix<double> configMatrix, Matrix<double> weightMatrix)
        {
            return (configMatrix.Transpose().Multiply(weightMatrix)).Multiply(configMatrix);
        }

        public Vector<double> CalculateK(Matrix<double> normalMatrix, Vector<double> inaccuracies)
        {// Using PseudoInverse in case of determinants approaching 0 -> numerically unstable inverse
            //Console.WriteLine("=========================");
            //Console.WriteLine($"Condition number: {normalMatrix.ConditionNumber()}");
            //Console.WriteLine("=========================");
            return -normalMatrix.PseudoInverse().Multiply(inaccuracies);
        }

        public Vector<double> CalculatePV(Vector<double> kVector, Matrix<double> confMatrix)
        {
            Vector<double> pvMatrix = vectorBuilder.Dense(Measurements.Count);
            foreach (TMeasurement meas in Measurements)
            {
                int i = GetMeasurementIndex(meas);
                Vector<double> currRow = confMatrix.Row(i);
                pvMatrix[i] = currRow.DotProduct(kVector);
            }
            return pvMatrix;
        }

        public List<List<TMeasurement>> AsignMeasurementsToTraverses(List<TMeasurement> measurements)
        {
            List<List<TMeasurement>> assignedMeasurements = new();
            foreach (List<PointBase> t in DistinctTraverses)
            {
                List<TMeasurement> currRes = AssignMeasurementsToTraverse(t, measurements);
                assignedMeasurements.Add(currRes);
            }
            return assignedMeasurements;
        }

        /// <summary>
        /// Generates a list with measurements depending on the list of points provided.
        /// The points are treated as graph vertices while the result is a list with the edges.
        /// </summary>
        /// <param name="traverse">A list with the points in a linearly independent path</param>
        /// <param name="measurements">A list containing all the measurements</param>
        /// <returns></returns>
        public List<TMeasurement> AssignMeasurementsToTraverse(List<PointBase> traverse, List<TMeasurement> measurements)
        {
            List<TMeasurement> processedTraverse = new();
            for (int index = 0; index < traverse.Count - 1; index++)
            {
                PointBase fromPoint = traverse[index];
                PointBase toPoint = traverse[index + 1];

                TMeasurement currMeas = FindMeasurementByPoints(fromPoint, toPoint, measurements);
                processedTraverse.Add(currMeas);
            }
            return processedTraverse;
        }

        public TMeasurement FindMeasurementByPoints(PointBase p1, PointBase p2, List<TMeasurement> measurements)
        {
            IEnumerable<TMeasurement> positiveMeas = measurements.Where(m => m.FromPoint == p1 && m.ToPoint == p2);
            IEnumerable<TMeasurement> negativeMeas = measurements.Where(m => m.FromPoint == p2 && m.ToPoint == p1);
            if (positiveMeas.Any())
            {
                return positiveMeas.First();
            }
            else if (negativeMeas.Any())
            {
                TMeasurement meas = negativeMeas.First();
                return CreateNegativeMeasurement(meas);
            }
            throw new InvalidDataException($"Measurement between points {p1.Number} and {p2.Number} not found!");
        }

        public int GetMeasurementIndex(TMeasurement meas)
        {
            foreach(int key in MeasurementIndices.Keys)
            {
                if (IsCorrespondingAlias(key, meas))
                {
                    return key;
                }
            }
            throw new InvalidDataException($"Couldn't find the alias for the measurement between {meas.FromPoint.Number} and {meas.ToPoint.Number}");
        }

        public bool IsLinkedTraverse(List<TMeasurement> traverse)
        {
            (PointBase startP, PointBase endP) = GetTravStartEndPoint(traverse);
            return startP.Number != endP.Number && KnownPointsCluster.IsKnownPoint(startP) && KnownPointsCluster.IsKnownPoint(endP);
        }

        public (PointBase, PointBase) GetTravStartEndPoint(List<TMeasurement> traverse)
        {
            TMeasurement firstMeas = traverse[0];
            TMeasurement lastMeas = traverse[^1];
            PointBase startP;
            PointBase endP;
            if (!firstMeas.IsReversed)
            {
                startP = firstMeas.FromPoint;
            }
            else
            {
                startP = firstMeas.ToPoint;
            }

            if (!lastMeas.IsReversed)
            {
                endP = lastMeas.ToPoint;
            }
            else
            {
                endP = lastMeas.FromPoint;
            }
            return (startP, endP);
        }

        /// <summary>
        /// Assigns an index to every measurement. Ensuring robust indexing system.
        /// </summary>
        private void AsignMeasurementsIndices()
        {
            foreach (TMeasurement meas in Measurements)
            {
                int nextIndex = MeasurementIndices.Count;
                MeasurementIndices[nextIndex] = meas;
            }
        }

        private HashSet<PointBase> GetPoinsSet()
        {
            HashSet<PointBase> set = new();
            foreach (List<PointBase> t in DistinctTraverses)
            {
                foreach(PointBase point in t)
                {
                    set.Add(point);
                }
            }
            return set;
        }

        private bool IsCorrespondingAlias(int key, TMeasurement meas)
        {
            return MeasurementIndices[key].FromPoint == meas.FromPoint &&
                    MeasurementIndices[key].ToPoint == meas.ToPoint;
        }

        private void ValidateAdjustmentResult(Vector<double> adjustedInaccuracies)
        {
            foreach(double currValue in adjustedInaccuracies)
            {
                if (Math.Round(currValue, RequiredDecimalPrecision) > 0)
                {
                    throw new IncorrectAdjustmentResultException("An adjusted correction exceeds the allowed threshold!");
                }
            }
        }
    }

    public class AdjustmentResult<TMeasurement, TAdjustedPoint>
        where TMeasurement : IEdge<PointBase, TMeasurement>
        where TAdjustedPoint : PointBase
    {
        List<TAdjustedPoint> AdjustedPoints { get; set; }

        List<TMeasurement> AdjustedMeasurements { get; set; }

        Vector<double> AdjustedCorrections { get; set; }

        public AdjustmentResult(List<TAdjustedPoint> adjustedPoints, 
            List<TMeasurement> adjustedMeasurements, 
            Vector<double> adjustedCorrections)
        {
            AdjustedPoints = adjustedPoints;
            AdjustedMeasurements = adjustedMeasurements;
            AdjustedCorrections = adjustedCorrections;
        }
    }
}
