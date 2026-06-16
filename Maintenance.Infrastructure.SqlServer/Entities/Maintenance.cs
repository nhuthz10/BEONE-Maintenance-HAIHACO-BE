using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Entities
{
    public class Maintenances
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string DocNo { get; set; }

        public int MtnType { get; set; }
        public string ItemCode { get; set; }
        public string? FactoryCode { get; set; }
        public string? FactoryName { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? PlannedCompletionDate { get; set; }
        public string? RequestUserCode { get; set; }
        public string? RequestUserName { get; set; }
        public string? Department { get; set; }
        public string? DepartmentDes { get; set; }
        public string? AssignUser { get; set; }
        public string? AssignUserName { get; set; }
        public string? AssignUserDepartment { get; set; }
        public string? AssignUserDepartmentDes { get; set; }
        public int ApproveStatus { get; set; }
        public string? RejectedReason { get; set; }
        public int Status { get; set; }
        public string? Remark { get; set; }
        public string? Dscription { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public ICollection<MaintenanceDocs>? MaintenanceDocs { get; set; }
        public ICollection<MaintenenceAttachments>? MaintenenceAttachments { get; set; }
        public ICollection<MaintenenceCheckLists>? MaintenenceCheckLists { get; set; }
        public ICollection<MaintenenceSpareParts>? MaintenenceSpareParts { get; set; }
    }
}
