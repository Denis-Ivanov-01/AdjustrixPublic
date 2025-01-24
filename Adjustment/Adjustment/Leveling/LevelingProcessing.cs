using Adjustment.Extensions;
using Adjustment.Project;
using Adjustment.Reports.Leveling;

namespace Adjustment
{
    public class LevelingProcessing : NetworkAdjuster<HeightDelta, LevelingAdjustment>
    {
        private readonly AdjustmentResult<HeightDelta, AdjustedBenchmark> AdjustmentResult;
        private readonly LevelingProject project;

        public LevelingProcessing(LevelingProject project, AdjustmentStatusDelegate? messageDelegate = null) : base(project.HeightDifferences, messageDelegate)
        {
            this.AdjustmentResult = PerformAdjustment();
            this.project = project;
        }

        public override void GeneratePdfReport(string directory)
        {
            LevelingReportPdf detailedReport = new(this.AdjustmentResult, this.project);
            detailedReport.Generate(directory);
        }

        public override void GenerateCsvReport(string directory)
        {
            AdjustedPointsReport benchmarksReport = new(this.AdjustmentResult.AdjustedPoints);
            benchmarksReport.Generate(directory);
        }
    }
}
