using System;
using System.Collections.Generic;

#nullable disable

namespace ManagerOrder.Models.Entities
{
    public partial class RegisterCustomer
    {
        public long Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerInitials { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public long? IsDelete { get; set; }
    }
}
