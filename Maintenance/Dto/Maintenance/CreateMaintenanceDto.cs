namespace Maintenance.Dto.Maintenance
{
    public class CreateMaintenanceDto
    {
        public int MtnType { get; set; }
        public string ItemCode { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime? MachineStopTime { get; set; }
        public DateTime? PlannedCompletionDate { get; set; }
        public string? Dscription { get; set; }
        public AssignToDto AssignTo { get; set; }
        public List<CreateMaintenenceAttachmentDto>? Attachments { get; set; }
    }

    public class CreateMaintenenceAttachmentDto
    {
        public int Order { get; set; }
        public IFormFile File { get; set; }
    }

    public class AssignToDto
    {
        public string UserCode { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }
        public string? DepartmentDes { get; set; }
        public string? Position { get; set; }
    }
}
