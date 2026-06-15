using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.SyncData
{
    public class EquipmentViewModel
    {
        public string ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? SubCat { get; set; }
        public string? UomCode { get; set; }
        public string? HHCCode { get; set; }
        public string IsActive { get; set; }
        public double? OnHand { get; set; }
    }
}
