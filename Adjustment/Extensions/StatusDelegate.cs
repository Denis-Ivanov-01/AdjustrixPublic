using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adjustment;

namespace Adjustment.Extensions
{
    public class StatusDelegate
    {
        private AdjustmentStatus currentStatus;

        public event Action<AdjustmentStatus> StatusChanged;

        public void ChangeStatus(AdjustmentStatus status)
        {
            currentStatus = status;
            StatusChanged?.Invoke(currentStatus);
        }
    }
}
