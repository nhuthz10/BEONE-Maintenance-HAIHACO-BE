using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Auth
{
    public class ViewLoginModel
    {
        public string token { get; set; }
        public UserViewModel userInfo { get; set; }
    }
}
