using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.Maintenance
{
    public class CreateGoodReceiptModel
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public List<CreateGoodReceiptDetailModel> Details { get; set; }
    }

    public class CreateGoodReceiptDetailModel
    {
        public string ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? UomCode { get; set; }
        public double? Quantity { get; set; }
    }
}
