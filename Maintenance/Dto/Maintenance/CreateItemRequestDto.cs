using Maintenance.Entities.Maintenance;

namespace Maintenance.Dto.Maintenance
{
    public class CreateItemRequestDto
    {
        public int Id { get; set; }
        public List<MaintenenceSparePartViewDto> SparePart { get; set; }
    }

    public class MaintenenceSparePartViewDto
    {
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? UomCode { get; set; }
        public double? Quantity { get; set; }
        public double? Stock { get; set; }
        public DateTime? RequiredTime { get; set; }
        public string? Note { get; set; }
    }
}
