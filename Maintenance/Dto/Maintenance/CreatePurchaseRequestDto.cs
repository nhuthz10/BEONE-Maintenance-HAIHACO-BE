using Maintenance.Entities.Maintenance;

namespace Maintenance.Dto.Maintenance
{
    public class CreatePurchaseRequestDto
    {
        public int Id { get; set; }
        public List<MaintenenceSparePartViewDto> SparePart { get; set; }
        public List<MaintenenceSparePartViewDto> PurchaseRequest { get; set; }
    }
}
