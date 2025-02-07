using AdjustrixBase.Adjustment.Base;
using AdjustrixBase.DataModels;
using AdjustrixBase.Extensions;
using AdjustrixBase.Project;
using AdjustrixBase.Reports.Leveling;

namespace AdjustrixBase.Adjustment.Leveling
{
    public class LevelingProcessing : NetworkAdjuster<HeightDelta, LevelingAdjustment>
    {
        private readonly AdjustmentResult<HeightDelta, AdjustedBenchmark> AdjustmentResult;
        private readonly LevelingProject project;
        private readonly Language language;

        private readonly StringsDictionary EN = new StringsDictionary
        {
            Title = "Leveling Adjustment Report",
            Contractor = "Contractor",
            Client = "Client",
            Project = "Project",
            Site = "Site",
            NumberOfMeasurements = "Number of measurements",
            NumberOfUnknownPoints = "Number of unknown points",
            NetworkRedundancy = "Network Redundancy",
            PerUnitVariance = "Per unit variance",
            AdjustedPointsTitle = "Adjusted points",
            ResidualsTitle = "Residuals",
            NumberOfPoint = "Number of Point",
            Elevation = "Elevation [m]",
            Variance = "Variance [mm]",
            Residual = "Residual",
            Value = "Value [mm]",
            Measurements = "Measurements",
            FromPoint = "From Point",
            ToPoint = "To Point"
        };

        private readonly StringsDictionary BG = new()
        {
            Title = "Доклад за изравнение",
            Contractor = "Изпълнител",
            Client = "Клиент",
            Project = "Проект",
            Site = "Обект",
            NumberOfMeasurements = "Брой измервания",
            NumberOfUnknownPoints = "Брой неизвестни точки",
            NetworkRedundancy = "Свръхизмервания",
            PerUnitVariance = "СКГ за единица тежест",
            AdjustedPointsTitle = "Изравнени точки",
            ResidualsTitle = "Несъвпадения",
            NumberOfPoint = "Номер на точка",
            Elevation = "Височина [м]",
            Variance = "СКГ [мм]",
            Residual = "Несъвпадение",
            Value = "Стойност [мм]",
            Measurements = "Измервания",
            FromPoint = "Начална точка",
            ToPoint = "Крайна точка"
        };

        public LevelingProcessing(LevelingProject project, AdjustmentStatusDelegate? messageDelegate = null, Language language = Language.Bulgarian) : base(project.HeightDifferences, messageDelegate)
        {
            this.language = language;
            AdjustmentResult = PerformAdjustment();
            this.project = project;
        }

        public override void GeneratePdfReport(string directory)
        {
            StringsDictionary dict = language == Language.Bulgarian ? BG : EN;
            LevelingReportPdf detailedReport = new(AdjustmentResult, project, dict);
            detailedReport.Generate(directory);
        }

        public override void GenerateCsvReport(string directory)
        {
            AdjustedPointsReport benchmarksReport = new(AdjustmentResult.AdjustedPoints);
            benchmarksReport.Generate(directory);
        }
    }
}
