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
    public class HistoryOrderDetailRepoController : Controller
    {
        HistoryOrderRepo orderRepo = new HistoryOrderRepo();
        HistoryOrderDetailRepo detailRepo = new HistoryOrderDetailRepo();
        RegisterProductRepo productRepo = new RegisterProductRepo();
        public IActionResult Index()
        {
            var session = HttpContext.Session.GetObject<RegisterUser>("user");
            if (session.Id <= 0)
            {
                return RedirectToAction("login", "home");
            }
            return View();
        }

        public JsonResult GetAll(int historyOrderId)
        {
            try
            {
                var session = HttpContext.Session.GetObject<RegisterUser>("user");
                if (session.Id <= 0)
                {
                    return Json(new
                    {
                        status = 0,
                        message = "Phiên đăng nhập đã hết.\nVui lòng đăng nhập lại!"
                    }, new JsonSerializerOptions());
                }

                HistoryOrder order = orderRepo.GetByID(historyOrderId);
                if (order == null)
                {
                    return Json(new
                    {
                        status = 0,
                        message = "Đơn đặt hàng không tồn tại.\nVui lòng kiểm tra lại!"
                    }, new JsonSerializerOptions());
                }
                else
                {
                    var listDetail = (from o in orderRepo.GetAll()
                                      join d in detailRepo.GetAll() on o.Id equals d.HistoryOrderId into t
                                      from d in t.DefaultIfEmpty()
                                      join p in productRepo.GetAll() on d.ProductId equals p.Id into t1
                                      from p in t1.DefaultIfEmpty()
                                      where o.Id == historyOrderId
                                      select new
                                      {
                                          ProductCode = p == null ? "" : p.ProductCode,
                                          ProductName = p == null ? "" : p.ProductName,
                                          Quantity = d.Qty,
                                          Price = d == null ? 0 : d.GiaBanRieng,
                                          TotalPrice = d.Qty * p.GiaBanChung
                                      }).ToList();

                    return Json(new
                    {
                        status = 1,
                        order = order,
                        detail = listDetail
                    }, new JsonSerializerOptions());

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
