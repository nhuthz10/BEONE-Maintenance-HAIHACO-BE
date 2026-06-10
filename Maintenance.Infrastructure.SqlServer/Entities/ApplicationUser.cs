using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Department { get; set; }
        public string? DepartmentDes { get; set; }
        public string? Position { get; set; }
        public bool IsActive { get; set; }
        public string? DeviceId { get; set; }
    }
}
