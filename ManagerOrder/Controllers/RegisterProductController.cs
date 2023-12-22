using ManagerOrder.CommonHelper;
using ManagerOrder.Models;
using ManagerOrder.Models.Entities;
using ManagerOrder.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManagerOrder.Controllers
{
    public class RegisterProductController : Controller
    {
        RegisterProductRepo productRepo = new RegisterProductRepo();
        UnitRepo unitRepo = new UnitRepo();
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

            ViewBag.Units = new SelectList(unitRepo.GetAll(), "Id", "Name");
            return View();
        }

        public JsonResult GetAll(string keyword)
        {
            try
            {
                var session = HttpContext.Session.GetObject<RegisterUser>("user");
                if (session.Id <= 0)
                {
                    return Json("Phiên đăng nhập đã hết.\nVui lòng đăng nhập lại!", new JsonSerializerOptions());
                }

                keyword = string.IsNullOrEmpty(keyword) ? "" : keyword.ToLower().Trim();
                var listProduct = (from p in productRepo.GetAll()
                                   join u in unitRepo.GetAll() on p.Unit equals u.Id into t
                                   from u in t.DefaultIfEmpty()
                                   where p.IsDelete != 1  && 
                                         (p.ProductCode.ToLower().Contains(keyword) || p.ProductName.ToLower().Contains(keyword) || keyword == "")
                                   select new
                                   {
                                       p.Id,
                                       p.ProductCode,
                                       p.ProductName,
                                       UnitName = u == null ? "" : u.Name,
                                       p.QtyInventory,
                                       p.QtyImport,
                                       p.QtyExport,
                                       ProductImportPrice = p.GiaNhap,
                                       WholesalePrice = p.GiaBanChung
                                   }).OrderByDescending(x=>x.Id).ToList();

                return Json(listProduct, new JsonSerializerOptions());
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public JsonResult GetByID(long id)
        {
            try
            {
                var session = HttpContext.Session.GetObject<RegisterUser>("user");
                if (session.Id <= 0)
                {
                    return Json("Phiên đăng nhập đã hết.\nVui lòng đăng nhập lại!", new JsonSerializerOptions());
                }

                RegisterProduct product = productRepo.GetByID(id);
                return Json(product, new JsonSerializerOptions());
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        public JsonResult SaveData([FromBody] RegisterProduct register)
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
                    return Json(new { status = 0, message = "Bạn không có quyền cập nhật sản phẩm!" }, new JsonSerializerOptions());
                }

                var validate = productRepo.CheckValidate(register);
                int status = (int)validate.GetType().GetProperty("status").GetValue(validate);
                if (status == 1)
                {
                    RegisterProduct product = productRepo.GetByID(register.Id);
                    if (product == null)
                    {
                        register.QtyInventory = register.QtyImport = register.QtyExport = 0;
                        productRepo.Create(register);
                        return Json(new { status = 1, message = "Thêm thành công!" }, new JsonSerializerOptions());
                    }
                    else
                    {
                        product.ProductCode = register.ProductCode;
                        product.ProductName = register.ProductName;
                        product.Unit = register.Unit;
                        product.GiaNhap = register.GiaNhap;
                        product.GiaBanChung = register.GiaBanChung;

                        productRepo.Update(product);
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
                    return Json(new { status = 0, message = "Bạn không có quyền xóa sản phẩm!" }, new JsonSerializerOptions());
                }

                RegisterProduct product = productRepo.GetByID(id);
                if (product == null)
                {
                    return Json(new { status = 0, message = "Sản phẩm không tại!" }, new JsonSerializerOptions());
                }
                else
                {
                    product.IsDelete = 1;
                    productRepo.Update(product);
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
