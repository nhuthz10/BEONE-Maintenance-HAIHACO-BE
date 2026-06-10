using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Entities
{
    public class EquipmentSpareParts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        [ForeignKey(nameof(Equipment))]
        public long EquipmentId { get; set; }
        public Equipments Equipment { get; set; }

        public string ItemCode { get; set; }
        public string? UomCode { get; set; }
        public string? Quantity { get; set; }
        public string IsRequired { get; set; }
        public string IsActive { get; set; }
    }
}
