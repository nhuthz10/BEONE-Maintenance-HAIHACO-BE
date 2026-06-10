using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Entities
{
    public class MaintenenceAttachments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        [ForeignKey(nameof(Maintenance))]
        public long HeaderId { get; set; }
        public Maintenances Maintenance { get; set; }


        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public int? Order { get; set; }
    }
}
