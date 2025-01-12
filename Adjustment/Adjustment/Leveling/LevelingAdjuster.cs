
namespace Adjustment
{
    public class LevelingAdjuster : NetworkAdjuster<HeightDifference, LevelingAdjustment>
    {
        public LevelingAdjuster(List<HeightDifference> measurements) : base(measurements)
        {

        }

        public override void GeneratePDFReport()
        {
            throw new NotImplementedException();
        }
    }
}
