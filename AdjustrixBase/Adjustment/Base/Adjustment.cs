using AdjustrixBase.DataModels;
using AdjustrixBase.Extensions;
//using AdjustrixBase.Mathematics.LinearAlgebra;
using AdjustrixBase.Mathematics.Operations;
using AdjustrixBase.NetworkAnalysis;
using MathNet.Numerics.LinearAlgebra;

namespace AdjustrixBase.Adjustment.Base
{
    public abstract class Adjustment<TMeasurement, TAdjustedPoint>
        where TMeasurement : IEdge<PointBase>, IDirectedMeasurement, new()
        where TAdjustedPoint : AdjustedBenchmark
    {
        public readonly MatrixBuilder<double> matrixBuilder = Matrix<double>.Build;
        public readonly VectorBuilder<double> vectorBuilder = Vector<double>.Build;
        public readonly HashSet<PointBase> Points;
        public readonly List<List<PointBase>> DistinctTraverses;
        public List<TMeasurement> Measurements;
        public readonly List<List<TMeasurement>> AssignedApproxMeasurements = new();
        private readonly Dictionary<int, TMeasurement> MeasurementIndices = new();
        private readonly int Redundancy;
        protected abstract int RequireddoublePrecision { get; }

        public Adjustment(List<List<PointBase>> distinctTraverses, List<TMeasurement> measurements)
        {
            DistinctTraverses = distinctTraverses;
            Redundancy = distinctTraverses.Count;
            Measurements = measurements;
            AsignMeasurementsIndices();
            AssignedApproxMeasurements = AssignMeasurementsToTraverses(measurements);
            Points = GetPoinsSet();
        }

        public AdjustmentResult<TMeasurement, TAdjustedPoint> AdjustNetwork()
        {

            Matrix<double> configurationMatrix = CreateConfigurationMatrix();
            Matrix<double> reversedWeights = CreateReversedWeightMatrix();
            Matrix<double> normalMatrix = CalculateNormalMatrix(configurationMatrix, reversedWeights);
            //normalMatrix.SaveToFile(@"C:\Users\denis\Desktop\matrix_normal.txt");

            Matrix<double> inversedNormal = InverseMatrix(normalMatrix);
            //inversedNormal.PrintMatrix();
            //inversedNormal.SaveToFile(@"C:\Users\denis\Desktop\matrix.txt");

            Vector<double> initialResiduals = CalculateResidualsVector(AssignedApproxMeasurements);
            Vector<double> kVector = CalculateK(inversedNormal, initialResiduals);
            Vector<double> pvVector = CalculatePV(kVector, configurationMatrix);

            Vector<double> corrections = CalculateCorrections(pvVector, reversedWeights);

            double perUnitVariance = CalculatePerUnitVariance(pvVector, corrections) * 1000;
            Matrix<double> Qv = CalculateQv(reversedWeights, configurationMatrix, perUnitVariance, inversedNormal);
            Vector<double> correctionVariances = CalculateCorrectionVariances(Qv, perUnitVariance);
            Vector<double> measurementVariances = CalculateMeasurementVariances(perUnitVariance, Qv);

            List<TMeasurement> adjustedMeasurements = CalculateAdjustedMeasurements(corrections);
            Vector<double> adjustedResiduals = CalculateResidualsVector(AssignMeasurementsToTraverses(adjustedMeasurements));
            ValidateAdjustmentResult(adjustedResiduals);
            List<TAdjustedPoint> adjustedPoints = CalculateUnknownPoints(adjustedMeasurements, Qv, perUnitVariance);

            AdjustmentResult<TMeasurement, TAdjustedPoint> result = new
                (adjustedPoints,
                adjustedMeasurements,
                measurementVariances,
                corrections,
                correctionVariances,
                DistinctTraverses,
                initialResiduals,
                perUnitVariance);
            return result;
        }

        protected abstract TMeasurement CreateNegativeMeasurement(TMeasurement meas);

        protected abstract Matrix<double> CreateConfigurationMatrix();

        protected abstract Matrix<double> CreateReversedWeightMatrix();

        protected abstract Vector<double> CalculateResidualsVector(List<List<TMeasurement>> assignedMeasurements);

        protected abstract Vector<double> CalculateCorrections(Vector<double> pvVector, Matrix<double> weightMatrix);

        protected abstract List<TMeasurement> CalculateAdjustedMeasurements(Vector<double> corrections);

        protected abstract List<TAdjustedPoint> CalculateUnknownPoints(List<TMeasurement> adjustedMeasurements, Matrix<double> Qv, double perUnitVaruance);

        protected Matrix<double> InverseMatrix(Matrix<double> matrix)
        {
            // Using PseudoInverse in case of determinants approaching 0
            //This way we ensure numeric stability

            //Another possible approach is to check the condition number and use PseudoInverse
            //if the condition number is above some threshold
            //todo: use this approach if computational speed gets important!
            //first research what value of the condition number is a good threshold

            return matrix.PseudoInverse();
        }

        protected Matrix<double> CalculateNormalMatrix(Matrix<double> configMatrix, Matrix<double> weightMatrix)
        {
            return configMatrix.Transpose().Multiply(weightMatrix).Multiply(configMatrix);
        }

        protected Vector<double> CalculateK(Matrix<double> inversedNormal, Vector<double> residuals)
        {
            return -inversedNormal.Multiply(residuals);
        }

        protected Vector<double> CalculatePV(Vector<double> kVector, Matrix<double> confMatrix)
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

        protected double CalculatePerUnitVariance(Vector<double> PV, Vector<double> V)
        {
            double pvv_sum = 0;
            for (int i = 0; i < PV.Count; i++)
            {
                pvv_sum += PV[i] * V[i];
            }
            return Math.Sqrt(pvv_sum / Redundancy);
        }

        protected Matrix<double> CalculateQv(Matrix<double> reversedWeights,
            Matrix<double> configMatrix,
            double perUnitVariance,
            Matrix<double> inversedNormal)
        {
            // I am not aware of a specific name for this matrix :(
            Matrix<double> placeholderMatrix = reversedWeights.Multiply(configMatrix);

            Matrix<double> Kk = inversedNormal * perUnitVariance;

            Matrix<double> Kv = placeholderMatrix.Multiply(Kk).Multiply(placeholderMatrix.Transpose());

            Matrix<double> Qv = Kv * (1 / perUnitVariance);
            return Qv;
        }

        protected Vector<double> CalculateCorrectionVariances(Matrix<double> Qv, double perUnitVariance)
        {
            Vector<double> variances = vectorBuilder.Dense(Qv.ColumnCount);
            for (int i = 0; i < variances.Count; i++)
            {
                variances[i] = perUnitVariance * Math.Sqrt(Qv[i, i]);
            }
            return variances;
        }

        protected Vector<double> CalculateMeasurementVariances(double perUnitVariance, Matrix<double> Qv)
        {
            Vector<double> variances = vectorBuilder.Dense(Qv.ColumnCount);
            for (int i = 0; i < variances.Count; i++)
            {
                variances[i] = perUnitVariance * Math.Sqrt(Qv[i, i]);
            }
            return variances;
        }

        protected List<List<TMeasurement>> AssignMeasurementsToTraverses(List<TMeasurement> measurements)
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
        protected List<TMeasurement> AssignMeasurementsToTraverse(List<PointBase> traverse, List<TMeasurement> measurements)
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

        protected TMeasurement FindMeasurementByPoints(PointBase p1, PointBase p2, List<TMeasurement> measurements)
        {
            IEnumerable<TMeasurement> positiveMeas = measurements.Where(
                m => m.FromPoint.Number == p1.Number && m.ToPoint.Number == p2.Number);
            IEnumerable<TMeasurement> negativeMeas = measurements.Where(
                m => m.FromPoint.Number == p2.Number && m.ToPoint.Number == p1.Number);
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

        protected int GetMeasurementIndex(TMeasurement meas)
        {
            foreach (int key in MeasurementIndices.Keys)
            {
                if (IsCorrespondingAlias(key, meas))
                {
                    return key;
                }
            }
            throw new InvalidDataException($"Couldn't find the alias for the measurement between {meas.FromPoint.Number} and {meas.ToPoint.Number}");
        }

        protected bool IsLinkedTraverse(List<TMeasurement> traverse)
        {
            (PointBase startP, PointBase endP) = GetTravStartEndPoint(traverse);
            return startP.Number != endP.Number && KnownPointsCluster.IsKnownPoint(startP) && KnownPointsCluster.IsKnownPoint(endP);
        }

        protected (PointBase, PointBase) GetTravStartEndPoint(List<TMeasurement> traverse)
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
        protected void AsignMeasurementsIndices()
        {
            foreach (TMeasurement meas in Measurements)
            {
                int nextIndex = MeasurementIndices.Count;
                MeasurementIndices[nextIndex] = meas;
            }
        }

        protected HashSet<PointBase> GetPoinsSet()
        {//todo: Add them using the measurements, not traverses?
            HashSet<PointBase> set = new();
            foreach (List<PointBase> t in DistinctTraverses)
            {
                foreach (PointBase point in t)
                {
                    set.Add(point);
                }
            }
            return set;
        }

        protected bool IsCorrespondingAlias(int key, TMeasurement meas)
        {
            return MeasurementIndices[key].FromPoint == meas.FromPoint &&
                    MeasurementIndices[key].ToPoint == meas.ToPoint;
        }

        protected void ValidateAdjustmentResult(Vector<double> adjustedResiduals)
        {
            foreach (double currValue in adjustedResiduals)
            {
                if (Math.Round(currValue, RequireddoublePrecision) > 0)
                {
                    throw new IncorrectAdjustmentResultException("An adjusted correction exceeds the allowed threshold!");
                }
            }
        }
    }

    public class AdjustmentResult<TMeasurement, TAdjustedPoint>
        where TMeasurement : IEdge<PointBase>
        where TAdjustedPoint : AdjustedBenchmark
    {
        public Vector<double> Residuals { get; set; }

        public List<TAdjustedPoint> AdjustedPoints { get; set; }

        public List<TMeasurement> AdjustedMeasurements { get; set; }

        public Vector<double> MeasurementVariances { get; set; }

        public Vector<double> AdjustedCorrections { get; set; }

        public Vector<double> CorrectionVariances { get; set; }

        public List<List<PointBase>> DistinctTraverses { get; set; }

        public double PerUnitVariance { get; set; }

        public AdjustmentResult(List<TAdjustedPoint> adjustedPoints,
            List<TMeasurement> adjustedMeasurements,
            Vector<double> measurementsVariances,
            Vector<double> adjustedCorrections,
            Vector<double> correctionVariances,
            List<List<PointBase>> distinctTravs,
            Vector<double> residuals,
            double perUnitVariance)
        {
            AdjustedPoints = adjustedPoints;
            AdjustedMeasurements = adjustedMeasurements;
            MeasurementVariances = measurementsVariances;
            AdjustedCorrections = adjustedCorrections;
            DistinctTraverses = distinctTravs;
            Residuals = residuals;
            PerUnitVariance = perUnitVariance;
            CorrectionVariances = correctionVariances;
        }
    }
}
