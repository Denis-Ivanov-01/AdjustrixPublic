using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adjustment.Adjustment.Leveling
{
    public class LevelingProcessing
    {
        private List<HeightDifference> measurements;

        public List<HeightDifference> HeightDifferences
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
            adjuster.PerformAdjustment();
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
