using Maintenance.Entities.Maintenance;

namespace Maintenance.Dto.Maintenance
{
    public class UpdateMaintenanceStatusDto
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public string? Remark { get; set; }
        public List<CreateMaintenenceAttachmentDto>? Attachments { get; set; }
    }
}
