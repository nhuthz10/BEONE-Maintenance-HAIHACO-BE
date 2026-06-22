using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Sap
{
    public class SapDraftResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int? DocEntry { get; set; }
        public int? DocNum { get; set; }
    }
}
