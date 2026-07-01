using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Notification
{
    public class CreateOrUpdateDeviceTokenModel
    {
        public string AccountId { get; set; }
        public string DeviceId { get; set; }
        public string Token { get; set; }
        public string Platform { get; set; }
    }
}
