using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Auth
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Version { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public string? DeviceVersion { get; set; }
    }
}
