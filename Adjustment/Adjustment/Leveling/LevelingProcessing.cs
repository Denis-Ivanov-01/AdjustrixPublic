namespace Adjustment.Adjustment.Leveling
{
    public class LevelingProcessing
    {
        private List<HeightDelta> measurements;

        public List<HeightDelta> HeightDifferences
        {
            get
            {
                if (measurements == null)
                {
                    throw new ArgumentNullException();
                }
                return measurements;
            }
        }

        public void PerformAdjustment()
        {
            LevelingAdjuster adjuster = new(HeightDifferences);
            AdjustmentResult<HeightDelta, AdjustedBenchmark> res = adjuster.PerformAdjustment();
        }

        public void LoadJson(string jsonPath)
        {
            JSONLevelingData data = new(jsonPath);
            measurements = data.HeightDifferences;
        }

        public void LoadExcel(string excelPath)
        {
            throw new NotImplementedException();
        }
    }
}
