using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesApp.ViewModels
{
    public class SalesItemVM
    {
        public int SaleItemId { get; set; }
        public int? SaleMasterId { get; set; }
        public int? ItemId { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Vat { get; set; }
        public decimal? Amount { get; set; }
        public string ItemName { get; set; }
    }
}
