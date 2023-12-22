using ManagerOrder.Repo;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using ManagerOrder.Models.Entities;
using ManagerOrder.CommonHelper;
using System.Diagnostics;
using ManagerOrder.Models;

namespace ManagerOrder.Controllers
{
    public class HistoryOrderController : Controller
    {
        HistoryOrderRepo orderRepo = new HistoryOrderRepo();
        RegisterCustomerRepo customerRepo = new RegisterCustomerRepo();
        public IActionResult Index()
        {
            var session = HttpContext.Session.GetObject<RegisterUser>("user");
            if (session.Id <= 0)
            {
                return RedirectToAction("login", "home");
            }

            //if (session.IsAdmin != 1)
            //{
            //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            //}

            return View();
        }

        public JsonResult GetAll(int deliverystatus, string keyword)
        {
            try
            {
                var session = HttpContext.Session.GetObject<RegisterUser>("user");
                if (session.Id <= 0)
                {
                    return Json("Phiên đăng nhập đã hết.\nVui lòng đăng nhập lại!", new JsonSerializerOptions());
                }

                keyword = string.IsNullOrEmpty(keyword) ? "" : keyword;
                var listOrder = (from order in orderRepo.GetAll()
                                 join customer in customerRepo.GetAll() on order.CustomerId equals customer.Id into t
                                 from customer in t.DefaultIfEmpty()
                                 where (Convert.ToInt32(order.DeliveryStatus) == deliverystatus || deliverystatus == -1) &&
                                       (order.OrderCode.ToLower().Contains(keyword.ToLower().Trim()) ||
                                        customer.CustomerName.ToLower().Contains(keyword.ToLower().Trim()) ||
                                        customer.CustomerAddress.ToLower().Contains(keyword.ToLower().Trim()) ||
                                        keyword == "")
                                 select new
                                 {
                                     order.Id,
                                     ShipperId = Convert.ToInt32(order.ShipperId),
                                     IsApproved = Convert.ToInt32(order.IsApproved),
                                     DeliveryStatus = Convert.ToInt32(order.DeliveryStatus) ,
                                     IsFullPayment = Convert.ToInt32(order.IsFullPayment),
                                     order.OrderCode,
                                     order.CustomerId,
                                     TotalIntoMoney = order.TongTien,
                                     CustomerPayment = order.TienKhachTra,
                                     MoneyOwedCustomer = order.TienKhachNo,
                                     order.CreatedDate,
                                     CustomerCode = customer == null ? "" : customer.CustomerCode,
                                     CustomerName = customer == null ? "" : customer.CustomerName,
                                     CustomerPhone = customer == null ? "" : customer.CustomerPhone,
                                     CustomerAddress = customer == null ? "" : customer.CustomerAddress,
                                 }).OrderByDescending(x => Convert.ToDateTime(x.CreatedDate)).ToList();
                return Json(listOrder, new JsonSerializerOptions());

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Update([FromBody] HistoryOrder historyOrder)
        {
            try
            {
                var session = HttpContext.Session.GetObject<RegisterUser>("user");
                if (session.Id <= 0)
                {
                    return Json("Phiên đăng nhập đã hết.\nVui lòng đăng nhập lại!", new JsonSerializerOptions());
                }
                else if (session.TypeId == 2)
                {
                    return Json($"Bạn không thể giao hàng!", new JsonSerializerOptions());
                }

                HistoryOrder order = orderRepo.GetByID(historyOrder.Id);
                if (order == null)
                {
                    return Json($"Đơn hàng không tồn tại!", new JsonSerializerOptions());
                }
                else if (order.IsApproved == 0)
                {
                    return Json($"Đơn hàng [{order.OrderCode}] chưa được duyệt.\nBạn không thể giao hàng!", new JsonSerializerOptions());
                }
                else if (order.DeliveryStatus == 1)
                {
                    return Json($"Đơn hàng [{order.OrderCode}] đã được giao thành công.\nBạn không thể giao hàng!", new JsonSerializerOptions());
                }

                order.DeliveryStatus = 1;//Giao hàng thành công
                order.TienKhachNo = order.TongTien - historyOrder.TienKhachTra;
                order.IsFullPayment = historyOrder.TienKhachTra <= 0 ? 0 : (historyOrder.TienKhachTra < order.TongTien ? 1 : 2);

                return Json(orderRepo.Update(order), new JsonSerializerOptions());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
