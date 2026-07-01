using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Entities
{
    public class MaintenenceCheckLists
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        [ForeignKey(nameof(Maintenance))]
        public long HeaderId { get; set; }
        public Maintenances Maintenance { get; set; }


        public string? JobType { get; set; }
        public string? CheckItem { get; set; }
        public string? EvaluationStandard { get; set; }
        public string? DataType { get; set; }
        public string? CheckResult { get; set; }
        public string? EquipmentStatus { get; set; }
        public string? Situation { get; set; }
        public string? Solution { get; set; }
        public string? Remark { get; set; }
    }
}
