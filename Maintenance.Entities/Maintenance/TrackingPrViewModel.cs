using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Maintenance
{
    public class TrackingPrViewModel
    {
        public List<TrackingPrDetailViewModel> Item {  get; set; }
        public List<TrackingPrDetailViewModel> Service { get; set; }
    }

    public class TrackingPrDetailViewModel
    {
        public string Type { get; set; }
        public string Key { get; set; }
        public int Process { get; set; }
        public string? Value { get; set; }
        public string? DocNo { get; set; }
        public string? User { get; set; }
        public string? Department { get; set; }
        public DateTime? Time { get; set; }
    }
}
