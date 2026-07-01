using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Entities
{
    public class NotificationLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string AccountId { get; set; }
        public string DeviceId { get; set; }
        public string FcmToken { get; set; }
        public string? Platform { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string? RefType { get; set; }
        public string? DocEntry { get; set; }
        public int? DocNum { get; set; }
        public string? DataJson { get; set; }
        public int Status { get; set; }
        public int RetryCount { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
    }
}
