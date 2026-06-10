using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Entities
{
    public class MaintenanceDocDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        [ForeignKey(nameof(MaintenanceDoc))]
        public long DocId { get; set; }
        public MaintenanceDocs MaintenanceDoc { get; set; }


        public string? ItemCode { get; set; }
        public string? Description { get; set; }
        public double? Quantity { get; set; }
        public string? UomCode { get; set; }
    }
}
