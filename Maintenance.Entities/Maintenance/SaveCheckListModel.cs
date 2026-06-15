using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Maintenance
{
    public class SaveCheckListModel
    {
        public long? Id { get; set; }
        public long MaintenanceId { get; set; }
        public bool IsDeleted { get; set; }
        public string? JobType { get; set; }
        public string? CheckItem { get; set; }
        public string? EvaluationStandard { get; set; }
        public string? CheckResult { get; set; }
        public string? EquipmentStatus { get; set; }
        public string? Remark { get; set; }
    }
}
