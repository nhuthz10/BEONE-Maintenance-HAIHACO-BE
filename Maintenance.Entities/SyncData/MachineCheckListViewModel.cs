using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.SyncData
{
    public class MachineCheckListViewModel
    {
        public string Code { get; set; }
        public int? LineId { get; set; }
        public string? Task { get; set; }
        public string? TaskGroup { get; set; }
        public string? Situation { get; set; }
        public string? Solution { get; set; }
        public string? Assignee { get; set; }
        public int? Duration { get; set; }
        public double? Maintenance { get; set; }
        public string? Required { get; set; }
        public string? EvaluationStandard { get; set; }
    }
}
