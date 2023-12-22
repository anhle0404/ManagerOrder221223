using System;
using System.Collections.Generic;

#nullable disable

namespace ManagerOrder.Models.Entities
{
    public partial class RegisterProduct
    {
        public long Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public long? Unit { get; set; }
        public long? QtyInventory { get; set; }
        public long? QtyImport { get; set; }
        public long? QtyExport { get; set; }
        public double? GiaNhap { get; set; }
        public double? GiaBanChung { get; set; }
        public long? IsDelete { get; set; }
    }
}
