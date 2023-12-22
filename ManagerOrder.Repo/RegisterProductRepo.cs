using ManagerOrder.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerOrder.Repo
{
    public class RegisterProductRepo:GenericRepo<RegisterProduct>
    {
        public object CheckValidate(RegisterProduct register)
        {
            if (string.IsNullOrEmpty(register.ProductCode.Trim()))
            {
                return new
                {
                    status = 0,
                    message = "Vui lòng nhập Mã sẩn phẩm!"
                };
            }
            else
            {
                var listProducts = GetAll().Where(x => x.ProductCode.ToLower().Trim() == register.ProductCode.ToLower().Trim() && x.Id != register.Id && x.IsDelete != 1).ToList();
                if (listProducts.Count > 0)
                {
                    return new
                    {
                        status = 0,
                        message = $"Mã sẩn phẩm [{register.ProductCode}] đã tồn tại!"
                    };
                }
            }

            if (string.IsNullOrEmpty(register.ProductName.Trim()))
            {
                return new
                {
                    status = 0,
                    message = "Vui lòng nhập Tên sẩn phẩm!"
                };
            }

            if (register.Unit <= 0)
            {
                return new
                {
                    status = 0,
                    message = "Vui lòng nhập Đơn vị!"
                };
            }

            if (register.GiaNhap <= 0)
            {
                return new
                {
                    status = 0,
                    message = "Vui lòng nhập Giá nhập!"
                };
            }

            if (register.GiaBanChung <= 0)
            {
                return new
                {
                    status = 0,
                    message = "Vui lòng nhập Giá bán lẻ!"
                };
            }

            return new
            {
                status = 1,
                message = ""
            };
        }
    }
}
