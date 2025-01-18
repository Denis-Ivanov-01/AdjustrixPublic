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
        private StatusDelegate? messageDelegate;

        public NetworkAdjuster(List<TMeasurement> measurements, StatusDelegate? messageDelegate=null)
        {
            _measurements = measurements.Where(x => x.IsEnabled).ToList();
            this.messageDelegate = messageDelegate;
        }

        public abstract void Process(string directory);

        public abstract void GeneratePDFReport();

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
    }
}
