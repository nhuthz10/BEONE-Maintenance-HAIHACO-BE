namespace Maintenance.Dto.Maintenance
{
    public class CreatePurchaseRequestServiceDto
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string Content { get; set; }
        public DateTime TimeRequiredService { get; set; }
    }
}
