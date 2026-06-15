using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Maintenance
{
    public class CreatePurchaseRequestServiceModel
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public string ServiceName { get; set; }
        public string Content { get; set; }
        public DateTime TimeRequiredService { get; set; }
    }
}
