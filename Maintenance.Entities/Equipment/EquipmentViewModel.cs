using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Equipment
{
    public class EquipmentViewModel
    {
        public long Id { get; set; }
        public string ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? Uom { get; set; }
        public string? FactoryCode { get; set; }
        public string? FactoryName { get; set; }
        public string? Line { get; set; }
        public string? Area { get; set; }
        public string? Section { get; set; }
        public string? SubCSection { get; set; }
        public double? Onhand { get; set; }
        public string? Manufacturer { get; set; }
        public string? Origin { get; set; }
        public string? Model { get; set; }
        public int? MnfYear { get; set; }
        public DateTime? UsedDate { get; set; }
        public string? MaintCycle { get; set; }
        public string? MaintCycleType { get; set; }
        public string? EquipManager { get; set; }
        public string? Power { get; set; }
        public string? Voltage { get; set; }
        public string? LastMaintBy { get; set; }
        public DateTime? LastMaintDate { get; set; }
        public DateTime? NextMaintDate { get; set; }
        public string IsActive { get; set; }
        public int? ReminderDays { get; set; }
        public List<EquipmentCheckListViewModel>? CheckLists { get; set; }
        public List<EquipmentSparePartViewModel>? SpareParts { get; set; }
    }

    public class EquipmentCheckListViewModel
    {
        public string? JobType { get; set; }
        public string? CheckItem { get; set; }
        public string? EvaluationStandard { get; set; }
        public string PassStatus { get; set; }
        public string? Remark { get; set; }
        public string IsActive { get; set; }
    }

    public class EquipmentSparePartViewModel
    {
        public string ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? UomCode { get; set; }
        public double? Quantity { get; set; }
        public string IsRequired { get; set; }
        public string IsActive { get; set; }
    }
}
