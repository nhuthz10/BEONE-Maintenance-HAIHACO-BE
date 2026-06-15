using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Maintenance
{
    public class UpdateMaintenanceStatusModel
    {
        public string AccountId { get; set; }
        public int Id { get; set; }
        public int Status { get; set; }
        public string? Remark { get; set; }
        public List<CreateMaintenenceAttachmentModel>? Attachments { get; set; }
    }
}
