using Adjustment.Extensions;

namespace Adjustment
{
    public enum AdjustmentStatus
    {
        DoingNothing,
        PreparingNetworkData,
        AnalyzingNetwork,
        CalculatingAdjustment,
        CreatingReports
    }

    public abstract class NetworkAdjuster<TMeasurement, TAdjustment>
        where TMeasurement : IEdge<PointBase, TMeasurement>, IDirectedMeasurement,
        IDeactivatable, new()
        where TAdjustment : Adjustment<TMeasurement, AdjustedBenchmark>
    {
        private readonly List<TMeasurement> _measurements;
        private readonly AdjustmentStatusDelegate? messageDelegate;

        private readonly DateTime startTime;

        public NetworkAdjuster(List<TMeasurement> measurements, AdjustmentStatusDelegate? messageDelegate = null)
        {
            startTime = DateTime.Now;
            _measurements = measurements.Where(x => x.IsEnabled).ToList();
            this.messageDelegate = messageDelegate;
        }

        public virtual void Process(string directory)
        {
            UpdateStatus(AdjustmentStatus.CreatingReports);

            GenerateCsvReport(directory);
            GeneratePdfReport(directory);

            DateTime endTime = DateTime.Now;
            UpdateDuration(endTime);
        }

        public abstract void GeneratePdfReport(string directory);

        public abstract void GenerateCsvReport(string directory);

        public AdjustmentResult<TMeasurement, AdjustedBenchmark> PerformAdjustment()
        {
            UpdateStatus(AdjustmentStatus.PreparingNetworkData);
            NetworkAnalyzer<TMeasurement> networkAnalyzer = new(_measurements);

            UpdateStatus(AdjustmentStatus.AnalyzingNetwork);
            List<List<PointBase>> distinctTraverses = networkAnalyzer.FindAllDistinctTraverses();

            // Using reflection to conform with the Open/Closed principle.
            // IMO the pros outweigh the cons in this case :)
            object[] args = new object[] { distinctTraverses, _measurements };
            TAdjustment adjustment = (TAdjustment)Activator.CreateInstance(typeof(TAdjustment), args)!;

            UpdateStatus(AdjustmentStatus.CalculatingAdjustment);
            return adjustment.AdjustNetwork();
            //todo: figure out what should be included in the reports and then figure out how to create
            // these reports in the most efficient way - in which class should that happen?
        }

        protected void UpdateStatus(AdjustmentStatus status)
        {
            if (messageDelegate != null)
            {
                messageDelegate.ChangeStatus(status);
            }
        }

        protected void UpdateDuration(DateTime endTime)
        {
            if (messageDelegate != null)
            {
                messageDelegate.ChangeDuration(endTime - startTime);
            }
        }
    }
}
