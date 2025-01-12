namespace LSA_Base
{
    public abstract class NetworkAdjuster<TMeasurement, TAdjustment>
        where TMeasurement : IEdge<PointBase, TMeasurement>, IDirectedMeasurement, new()
        where TAdjustment : Adjustment<TMeasurement, AdjustedPoint>
    {
        private readonly List<TMeasurement> _measurements;

        public NetworkAdjuster(List<TMeasurement> measurements)
        {
            _measurements = measurements;
        }

        public abstract void GeneratePDFReport();

        public void PerformAdjustment()
        {
            NetworkAnalyzer<TMeasurement> networkAnalyzer = new(_measurements);
            List<List<PointBase>> distinctTraverses = networkAnalyzer.FindAllDistinctTraverses();

            // Using reflection to conform with the Open/Closed principle.
            // IMO the pros outweigh the cons in this case :)
            object[] args = new object[] { distinctTraverses, _measurements };
            TAdjustment adjustment = (TAdjustment)Activator.CreateInstance(typeof(TAdjustment), args)!;
            adjustment.AdjustNetwork();
            //todo: figure out what should be included in the reports and then figure out how to create
            // these reports in the most efficient way - in which class should that happen?
        }
    }
}
