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
        HistoryOrderDetailRepo detailRepo = new HistoryOrderDetailRepo();
        RegisterCustomerRepo customerRepo = new RegisterCustomerRepo();
        RegisterProductRepo productRepo = new RegisterProductRepo();
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

        public JsonResult GetAll(DateTime dateStart, DateTime dateEnd, int deliverystatus, string keyword)
        {
            try
            {
                var session = HttpContext.Session.GetObject<RegisterUser>("user");
                if (session.Id <= 0)
                {
                    return Json("Phiên đăng nhập đã hết.\nVui lòng đăng nhập lại!", new JsonSerializerOptions());
                }

                dateStart = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
                dateEnd = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
                keyword = string.IsNullOrEmpty(keyword) ? "" : keyword;

                var listOrder = (from order in orderRepo.GetAll()
                                 join customer in customerRepo.GetAll() on order.CustomerId equals customer.Id into t
                                 from customer in t.DefaultIfEmpty()
                                 where (Convert.ToDateTime(order.CreatedDate) >= dateStart && Convert.ToDateTime(order.CreatedDate) <= dateEnd) &&
                                        (Convert.ToInt32(order.DeliveryStatus) == deliverystatus || deliverystatus == -1) &&
                                        (order.OrderCode.ToLower().Contains(keyword.ToLower().Trim()) ||
                                        customer.CustomerName.ToLower().Contains(keyword.ToLower().Trim()) ||
                                        customer.CustomerAddress.ToLower().Contains(keyword.ToLower().Trim()) ||
                                        keyword == "")
                                 select new
                                 {
                                     order.Id,
                                     ShipperId = Convert.ToInt32(order.ShipperId),
                                     IsApproved = Convert.ToInt32(order.IsApproved),
                                     DeliveryStatus = Convert.ToInt32(order.DeliveryStatus),
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
                else if (order.IsApproved == 1)
                {
                    return Json($"Đơn hàng [{order.OrderCode}] đã được duyệt.\nBạn không thể giao hàng!", new JsonSerializerOptions());
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

        [HttpPost]
        public JsonResult Approved([FromBody] List<HistoryOrder> historyOrders)
        {
            try
            {

                var session = HttpContext.Session.GetObject<RegisterUser>("user");
                if (session.Id <= 0)
                {
                    return Json("Phiên đăng nhập đã hết.\nVui lòng đăng nhập lại!", new JsonSerializerOptions());
                }
                else if (session.TypeId != 1)
                {
                    if (historyOrders.Count <= 0)
                    {
                        return Json("Vui lòng chọn đơn hàng!", new JsonSerializerOptions());
                    }
                    string message = historyOrders.FirstOrDefault().IsApproved == 1 ? "duyệt" : "hủy duyệt";
                    return Json($"Bạn không thể {message} giao hàng!", new JsonSerializerOptions());
                }

                foreach (HistoryOrder item in historyOrders)
                {
                    HistoryOrder order = orderRepo.GetByID(item.Id);
                    if (order == null)
                    {
                        return Json("Đơn hàng không tồn tại!", new JsonSerializerOptions());
                    }

                    order.IsApproved = item.IsApproved;
                    orderRepo.Update(order);
                }

                return Json(1, new JsonSerializerOptions());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Delivery([FromBody] List<HistoryOrder> historyOrders)
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
                    string message = historyOrders.FirstOrDefault().DeliveryStatus == 1 ? "giao hàng" : "hủy giao hàng";
                    return Json($"Bạn không thể {message}!", new JsonSerializerOptions());
                }

                if (historyOrders.Count <= 0)
                {
                    return Json("Vui lòng chọn đơn hàng!", new JsonSerializerOptions());
                }

                foreach (HistoryOrder item in historyOrders)
                {
                    HistoryOrder order = orderRepo.GetByID(item.Id);
                    if (order == null)
                    {
                        return Json("Đơn hàng không tồn tại!", new JsonSerializerOptions());
                    }

                    order.DeliveryStatus = item.DeliveryStatus;
                    order.TienKhachTra = item.DeliveryStatus == 1 ? item.TienKhachTra : 0;
                    order.TienKhachNo = order.TongTien - order.TienKhachTra;
                    order.IsFullPayment = order.TienKhachTra <= 0 ? 0 : (order.TienKhachTra < order.TongTien ? 1 : 2);
                    orderRepo.Update(order);
                }

                return Json(1, new JsonSerializerOptions());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult GetListDelivery([FromBody] List<HistoryOrder> historyOrders)
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
                    if (historyOrders.Count <= 0)
                    {
                        return Json("Vui lòng chọn đơn hàng!", new JsonSerializerOptions());
                    }
                    string message = historyOrders.FirstOrDefault().DeliveryStatus == 1 ? "giao hàng" : "hủy giao hàng";
                    return Json($"Bạn không thể {message}!", new JsonSerializerOptions());
                }

                if (historyOrders.Count <= 0)
                {
                    return Json("Vui lòng chọn đơn hàng!", new JsonSerializerOptions());
                }

                List<object> listDelivery = new List<object>();
                foreach (HistoryOrder item in historyOrders)
                {
                    HistoryOrder order = orderRepo.GetByID(item.Id);
                    if (order == null)
                    {
                        continue;
                    }
                    var details = (from o in orderRepo.GetAll()
                                      join d in detailRepo.GetAll() on o.Id equals d.HistoryOrderId into t
                                      from d in t.DefaultIfEmpty()
                                      join p in productRepo.GetAll() on d.ProductId equals p.Id into t1
                                      from p in t1.DefaultIfEmpty()
                                      where o.Id == item.Id
                                      select new
                                      {
                                          ProductCode = p == null ? "" : p.ProductCode,
                                          ProductName = p == null ? "" : p.ProductName,
                                          Quantity = d.Qty,
                                          Price = d == null ? 0 : d.GiaBanRieng,
                                          TotalPrice = d.Qty * p.GiaBanChung
                                      }).ToList();
                    var delivery = new
                    {
                        order = order,
                        detail = details
                    };

                    listDelivery.Add(delivery);
                }

                return Json(listDelivery, new JsonSerializerOptions());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
