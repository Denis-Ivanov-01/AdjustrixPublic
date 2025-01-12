
namespace LSA_Base
{
    public class GravimetricAdjuster : NetworkAdjuster<RelativeMeasurement, GravimetricAdjustment>
    {
        public GravimetricAdjuster(List<RelativeMeasurement> measurements) : base(measurements)
        {

        }

        public override void GeneratePDFReport()
        {
            throw new NotImplementedException();
        }
    }
}
