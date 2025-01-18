using Adjustment.Reports.Leveling;

namespace Adjustment
{
    public class LevelingProcessing : NetworkAdjuster<HeightDelta, LevelingAdjustment>
    {

        private AdjustmentResult<HeightDelta, AdjustedBenchmark> AdjustmentResult;

        public LevelingProcessing(List<HeightDelta> measurements) : base(measurements)
        {
            this.AdjustmentResult = PerformAdjustment();
        }

        public override void Process(string directory)
        {
            AdjustedPointsReport benchmarksReport = new(this.AdjustmentResult.AdjustedPoints);
            benchmarksReport.Generate(directory);
        }

        public override void GeneratePDFReport()
        {
            throw new NotImplementedException();
        }
    }
}
