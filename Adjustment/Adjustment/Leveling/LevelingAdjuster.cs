
using Adjustment.Reports;

namespace Adjustment
{
    public class LevelingAdjuster : NetworkAdjuster<HeightDelta, LevelingAdjustment>
    {

        public LevelingAdjuster(List<HeightDelta> measurements) : base(measurements)
        {
        }

        public override void GeneratePDFReport()
        {
            throw new NotImplementedException();
        }
    }
}
