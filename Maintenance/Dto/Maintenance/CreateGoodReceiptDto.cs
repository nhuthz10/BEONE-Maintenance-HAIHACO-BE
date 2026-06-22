using Maintenance.Entities.Maintenance;

namespace Maintenance.Dto.Maintenance
{
    public class CreateGoodReceiptDto
    {
        public int Id { get; set; }
        public List<CreateGoodReceiptDetailDto> Details { get; set; }
    }

    public class CreateGoodReceiptDetailDto
    {
        public string ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? UomCode { get; set; }
        public double? Quantity { get; set; }
    }
}
