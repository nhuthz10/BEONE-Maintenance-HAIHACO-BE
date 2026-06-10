namespace Maintenance.Dto.Auth
{
    public class LoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Version { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public string? DeviceVersion { get; set; }
    }
}
