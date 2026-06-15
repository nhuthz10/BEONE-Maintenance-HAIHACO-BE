using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Entities
{
    public class Equipments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int? ItemGroup { get; set; }
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
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? ReminderDays { get; set; }
        public int? IsNoti { get; set; }
        public ICollection<EquipmentCheckLists>? CheckLists { get; set; }
        public ICollection<EquipmentSpareParts>? SpareParts { get; set; }
    }
}
