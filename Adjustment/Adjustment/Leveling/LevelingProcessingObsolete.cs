namespace Adjustment.Adjustment.Leveling
{
    [Obsolete]
    public class LevelingProcessingObsolete
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
            //LevelingProcessing adjuster = new(HeightDifferences);
            //AdjustmentResult<HeightDelta, AdjustedBenchmark> res = adjuster.PerformAdjustment();
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
