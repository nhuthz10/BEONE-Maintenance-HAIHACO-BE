using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.SyncData
{
    public class MachineSparePartViewModel
    {
        public string Code { get; set; }
        public int? LineId { get; set; }
        public string ItemCode { get; set; }
        public string? Dscription { get; set; }
        public string? Uom { get; set; }
        public double? Quantity { get; set; }
        public int? Maintenance { get; set; }
        public string? Father { get; set; }

    }
}
