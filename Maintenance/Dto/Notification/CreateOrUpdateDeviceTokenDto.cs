namespace Maintenance.Dto.Notification
{
    public class CreateOrUpdateDeviceTokenDto
    {
        public string DeviceId { get; set; }
        public string Token { get; set; }
        public string Platform { get; set; }
    }
}
