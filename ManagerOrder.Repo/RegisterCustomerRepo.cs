using ManagerOrder.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerOrder.Repo
{
    public class RegisterCustomerRepo : GenericRepo<RegisterCustomer>
    {
        public object CheckValidate(RegisterCustomer customer)
        {
            if (string.IsNullOrEmpty(customer.CustomerCode))
            {
                return new { status = 0, message = "Vui lòng nhập Mã khách hàng!"};
            }
            else 
            {
                var customers = GetAll().Where(x => x.CustomerCode.Trim().ToLower() == customer.CustomerCode.Trim().ToLower() && x.Id != customer.Id).ToList();
                if (customers.Count > 0)
                {
                    return new { status = 0, message = $"Mã khách hàng [{customer.CustomerCode}] đã tồn tại!" };
                }
            }

            if (string.IsNullOrEmpty(customer.CustomerName))
            {
                return new { status = 0, message = "Vui lòng nhập Tên khách hàng!" };
            }

            if (string.IsNullOrEmpty(customer.CustomerPhone))
            {
                return new { status = 0, message = "Vui lòng nhập Số điện thoại!" };
            }

            if (string.IsNullOrEmpty(customer.CustomerAddress))
            {
                return new { status = 0, message = "Vui lòng nhập Địa chỉ!" };
            }

            return new { status = 1, message = "" };
        }
    }
}
