using System.Data;
using Adjustment.Extensions;
using Adjustment.Reports.Leveling;

namespace Adjustment
{
    public class LevelingProcessing : NetworkAdjuster<HeightDelta, LevelingAdjustment>
    {

        private AdjustmentResult<HeightDelta, AdjustedBenchmark> AdjustmentResult;

        public LevelingProcessing(List<HeightDelta> measurements, StatusDelegate? messageDelegate=null) : base(measurements, messageDelegate)
        {
            this.AdjustmentResult = PerformAdjustment();
        }

        public override void Process(string directory)
        {
            UpdateStatus(AdjustmentStatus.CreatingReports);
            AdjustedPointsReport benchmarksReport = new(this.AdjustmentResult.AdjustedPoints);
            benchmarksReport.Generate(directory);
            UpdateStatus(AdjustmentStatus.DoingNothing);
        }

        public override void GeneratePDFReport()
        {
            throw new NotImplementedException();
        }
    }
}
