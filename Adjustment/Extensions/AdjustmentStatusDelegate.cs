using AdjustrixBase.Adjustment.Base;

namespace AdjustrixBase.Extensions
{
    public class AdjustmentStatusDelegate
    {
        private AdjustmentStatus currentStatus;
        private TimeSpan processingTime;

        public event Action<AdjustmentStatus> StatusChanged;
        public event Action<TimeSpan> DurationChanged;

        public void ChangeStatus(AdjustmentStatus status)
        {
            currentStatus = status;
            StatusChanged?.Invoke(currentStatus);
        }

        public void ChangeDuration(TimeSpan duration)
        {
            processingTime = duration;
            DurationChanged?.Invoke(processingTime);
        }
    }
}
