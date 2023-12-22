using System;
using System.Collections.Generic;

#nullable disable

namespace ManagerOrder.Models.Entities
{
    public partial class RegisterUser
    {
        public long Id { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public long? TypeId { get; set; }
        public string TypeName { get; set; }
    }
}
