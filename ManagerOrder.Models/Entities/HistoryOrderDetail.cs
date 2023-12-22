using System;
using System.Collections.Generic;

#nullable disable

namespace ManagerOrder.Models.Entities
{
    public partial class HistoryOrderDetail
    {
        public long Id { get; set; }
        public long? HistoryOrderId { get; set; }
        public long? ProductId { get; set; }
        public long? Qty { get; set; }
        public double? GiaBanChung { get; set; }
        public double? GiaNhap { get; set; }
        public double? GiaBanRieng { get; set; }
    }
}
