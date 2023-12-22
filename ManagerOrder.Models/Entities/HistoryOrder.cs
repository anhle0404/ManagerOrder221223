using System;
using System.Collections.Generic;

#nullable disable

namespace ManagerOrder.Models.Entities
{
    public partial class HistoryOrder
    {
        public long Id { get; set; }
        public long? ShipperId { get; set; }
        public long? SalesStaffId { get; set; }
        public long? IsApproved { get; set; }
        public long? DeliveryStatus { get; set; }
        public long? IsFullPayment { get; set; }
        public string OrderCode { get; set; }
        public long? CustomerId { get; set; }
        public double? TongTien { get; set; }
        public double? TienKhachTra { get; set; }
        public double? TienKhachNo { get; set; }
        public string CreatedDate { get; set; }
    }
}
