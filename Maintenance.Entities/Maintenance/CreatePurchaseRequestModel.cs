using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Maintenance
{
    public class CreatePurchaseRequestModel
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public List<MaintenenceSparePartViewModel> SparePart { get; set; }
        public List<MaintenenceSparePartViewModel> PurchaseRequest { get; set; }
    }
}
