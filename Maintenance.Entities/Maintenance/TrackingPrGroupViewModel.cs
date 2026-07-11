using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Maintenance
{
    public class TrackingPrGroupViewModel
    {
        public string DocKey { get; set; }
        public List<TrackingPrDetailViewModel> Details { get; set; } = new();
    }
}
