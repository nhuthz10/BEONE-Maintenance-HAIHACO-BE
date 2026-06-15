using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.User
{
    public class UserViewModel
    {
        public string UserCode { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }
        public string? DepartmentDes { get; set; }
        public string? Position { get; set; }
    }
}
