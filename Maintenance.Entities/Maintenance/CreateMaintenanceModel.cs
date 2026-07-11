using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Maintenance
{
    public class CreateMaintenanceModel
    {
        public int MtnType { get; set; }
        public string ItemCode { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime? MachineStopTime { get; set; }
        public DateTime? PlannedCompletionDate { get; set; }
        public string? Dscription { get; set; }
        public string CreatedBy { get; set; }
        public List<CreateMaintenenceAttachmentModel>? Attachments { get; set; }
    }

    public class CreateMaintenenceAttachmentModel
    {
        public int Order { get; set; }
        public IFormFile File { get; set; }
    }
}
