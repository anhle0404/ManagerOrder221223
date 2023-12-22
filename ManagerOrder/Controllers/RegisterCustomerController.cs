using ManagerOrder.CommonHelper;
using ManagerOrder.Models.Entities;
using ManagerOrder.Repo;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManagerOrder.Controllers
{
    public class RegisterCustomerController : Controller
    {
        RegisterCustomerRepo customerRepo = new RegisterCustomerRepo();
        public IActionResult Index()
        {
            var session = HttpContext.Session.GetObject<RegisterUser>("user");
            if (session.Id <= 0)
            {
                return RedirectToAction("login", "home");
            }

            if (session.TypeId == 3)
            {
                return RedirectToAction("Authentication", "home");
            }

            return View();
        }

        public JsonResult GetAll(string keyword)
        {
            var session = HttpContext.Session.GetObject<RegisterUser>("user");
            if (session.Id <= 0)
            {
                return Json(new { status = 0, message = "Phiên đăng nhập đã hết.\nVui lòng đăng nhập lại!" }, new JsonSerializerOptions());
            }

            keyword = string.IsNullOrEmpty(keyword) ? "" : keyword.Trim().ToLower();
            var list = customerRepo.GetAll().Where(x => x.IsDelete != 1 &&
                                                    (x.CustomerCode.ToLower().Contains(keyword) ||
                                                     x.CustomerName.ToLower().Contains(keyword) ||
                                                     x.CustomerPhone.ToLower().Contains(keyword) ||
                                                     x.CustomerAddress.ToLower().Contains(keyword) || keyword == ""))
                                            .OrderByDescending(x=>x.Id)
                                            .ToList();
            return Json(list, new JsonSerializerOptions());
        }

        public JsonResult GetByID(int id)
        {
            try
            {
                var session = HttpContext.Session.GetObject<RegisterUser>("user");
                if (session.Id <= 0)
                {
                    return Json(new { status = 0, message = "Phiên đăng nhập đã hết.\nVui lòng đăng nhập lại!" }, new JsonSerializerOptions());
                }

                RegisterCustomer customer = customerRepo.GetByID(id);
                return Json(customer, new JsonSerializerOptions());

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public JsonResult SaveData([FromBody] RegisterCustomer register)
        {
            try
            {
                var session = HttpContext.Session.GetObject<RegisterUser>("user");
                if (session.Id <= 0)
                {
                    return Json(new { status = 0, message = "Phiên đăng nhập đã hết.\nVui lòng đăng nhập lại!" }, new JsonSerializerOptions());
                }
                else if (session.TypeId != 1) //Không phải admnin
                {
                    return Json(new { status = 0, message = "Bạn không có quyền cập nhật khách hàng!" }, new JsonSerializerOptions());
                }

                var validate = customerRepo.CheckValidate(register);
                int status = (int)validate.GetType().GetProperty("status").GetValue(validate);
                if (status == 1)
                {
                    RegisterCustomer customer = customerRepo.GetByID(register.Id);
                    if (customer == null)
                    {
                        customerRepo.Create(register);
                        return Json(new { status = 1, message = "Thêm thành công!" }, new JsonSerializerOptions());
                    }
                    else
                    {
                        customer.CustomerCode = register.CustomerCode.Trim();
                        customer.CustomerName = register.CustomerName.Trim();
                        customer.CustomerInitials = register.CustomerInitials.Trim();
                        customer.CustomerPhone = register.CustomerPhone.Trim();
                        customer.CustomerAddress = register.CustomerAddress.Trim();

                        customerRepo.Update(customer);
                        return Json(new { status = 1, message = "Cập nhật thành công!" }, new JsonSerializerOptions());
                    }
                }
                else
                {
                    return Json(validate, new JsonSerializerOptions());
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public JsonResult Delete(int id)
        {
            try
            {
                var session = HttpContext.Session.GetObject<RegisterUser>("user");
                if (session.Id <= 0)
                {
                    return Json(new { status = 0, message = "Phiên đăng nhập đã hết.\nVui lòng đăng nhập lại!" }, new JsonSerializerOptions());
                }
                else if (session.TypeId != 1) //Không phải admnin
                {
                    return Json(new { status = 0, message = "Bạn không có quyên xóa sản phẩm!" }, new JsonSerializerOptions());
                }

                RegisterCustomer customer = customerRepo.GetByID(id);
                if (customer == null)
                {
                    return Json(new { status = 0, message = "Khách hàng không tại!" }, new JsonSerializerOptions());
                }
                else
                {
                    customer.IsDelete = 1;
                    customerRepo.Update(customer);
                    return Json(new { status = 1, message = "Xóa thành công!" }, new JsonSerializerOptions());
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
