using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Entities
{
    public class MaintenanceDocs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        [ForeignKey(nameof(Maintenance))]
        public long HeaderId { get; set; }
        public Maintenances Maintenance { get; set; }


        public long? DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string? CardCode { get; set; }
        public string? DocType { get; set; }
        public int? ObjectType { get; set; }
        public string? Machine { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<MaintenanceDocDetails>? Details { get; set; }
    }
}
