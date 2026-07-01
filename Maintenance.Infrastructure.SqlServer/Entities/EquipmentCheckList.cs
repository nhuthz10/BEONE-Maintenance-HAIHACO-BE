using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Entities
{
    public class EquipmentCheckLists
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        [ForeignKey(nameof(Equipment))]
        public long EquipmentId { get; set; }
        public Equipments Equipment { get; set; }

        public int? LineId { get; set; }
        public string? JobType { get; set; }
        public string? CheckItem { get; set; }
        public string? Situation { get; set; }
        public string? Solution { get; set; }
        public string? EvaluationStandard { get; set; }
        public string PassStatus { get; set; }
        public string? Remark { get; set; }
        public string IsActive { get; set; }
    }
}
