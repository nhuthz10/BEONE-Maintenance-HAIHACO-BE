namespace Maintenance.Dto.Maintenance
{
    public class SaveCheckListDto
    {
        public long? Id { get; set; }
        public long MaintenanceId { get; set; }
        public bool IsDeleted { get; set; }
        public int? JobType { get; set; }
        public string? CheckItem { get; set; }
        public string? EvaluationStandard { get; set; }
        public string? CheckResult { get; set; }
        public string? EquipmentStatus { get; set; }
        public string? Remark { get; set; }
    }
}
