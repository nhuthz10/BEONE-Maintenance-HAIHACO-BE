namespace Maintenance.Dto.Maintenance
{
    public class CreateMaintenanceDto
    {
        public int MtnType { get; set; }
        public string ItemCode { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime? PlannedCompletionDate { get; set; }
        public string? Dscription { get; set; }
        public List<CreateMaintenenceAttachmentDto>? Attachments { get; set; }
    }

    public class CreateMaintenenceAttachmentDto
    {
        public int Order { get; set; }
        public IFormFile File { get; set; }
    }
}
