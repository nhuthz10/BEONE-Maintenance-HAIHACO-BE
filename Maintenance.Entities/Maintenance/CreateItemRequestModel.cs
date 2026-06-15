using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Maintenance
{
    public class CreateItemRequestModel
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public List<MaintenenceSparePartViewModel> SparePart { get; set; }
    }
}
