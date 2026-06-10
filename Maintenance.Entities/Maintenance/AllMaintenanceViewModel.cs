using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Maintenance
{
    public class AllMaintenanceViewModel
    {
        public List<MaintenanceViewModel> Maintenances { get; set; }
        public int TotalCount { get; set; }
        public int WaitExecutionCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }
    }
}
