using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Maintenance
{
    public class MaintenanceViewModel
    {
        public long Id { get; set; }
        public string DocNo { get; set; }
        public int MtnType { get; set; }
        public string ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? FactoryCode { get; set; }
        public string? FactoryName { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime PlannedCompletionDate { get; set; }
        public string? RequestUserCode { get; set; }
        public string? RequestUserName { get; set; }
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

        public List<MaintenanceDocViewModel>? MaintenanceDocs { get; set; }
        public List<MaintenenceAttachmentViewModel>? MaintenenceAttachments { get; set; }
        public List<MaintenenceCheckListViewModel>? MaintenenceCheckLists { get; set; }
        public List<MaintenenceSparePartViewModel>? MaintenenceSpareParts { get; set; }
    }

    public class MaintenanceDocViewModel
    {
        public long? DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string? CardCode { get; set; }
        public string? DocType { get; set; }
        public int? ObjectType { get; set; }
        public string? Machine { get; set; }
        public List<MaintenanceDocDetailViewModel>? Details { get; set; }
    }

    public class MaintenanceDocDetailViewModel
    {
        public string? ItemCode { get; set; }
        public string? Description { get; set; }
        public double? Quantity { get; set; }
        public string? UomCode { get; set; }
    }

    public class MaintenenceAttachmentViewModel
    {
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public int? Order { get; set; }
    }
    public class MaintenenceCheckListViewModel
    {
        public string? JobType { get; set; }
        public string? CheckItem { get; set; }
        public string? EvaluationStandard { get; set; }
        public string? DataType { get; set; }
        public string? CheckResult { get; set; }
        public string? EquipmentStatus { get; set; }
        public string? Remark { get; set; }
    }
    public class MaintenenceSparePartViewModel
    {
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? UomCode { get; set; }
        public double? Quantity { get; set; }
    }
}
