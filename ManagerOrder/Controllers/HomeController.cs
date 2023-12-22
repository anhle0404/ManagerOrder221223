using ManagerOrder.CommonHelper;
using ManagerOrder.Models;
using ManagerOrder.Models.Entities;
using ManagerOrder.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManagerOrder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        RegisterUserRepo userRepo = new RegisterUserRepo();
        RegisterProductRepo productRepo = new RegisterProductRepo();
        HistoryOrderRepo orderRepo = new HistoryOrderRepo();
        HistoryOrderDetailRepo orderDetailRepo = new HistoryOrderDetailRepo();
        UnitRepo unitRepo = new UnitRepo();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var session = HttpContext.Session.GetObject<RegisterUser>("user");
            if (session.Id <= 0)
            {
                return RedirectToAction("login", "home");
            }

            if (session.TypeId != 1)
            {
                return RedirectToAction("Authentication", "home");
            }


            return View();
        }

        public IActionResult Authentication()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        #region Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromForm] RegisterUser login)
        {
            try
            {
                RegisterUser user = userRepo.GetAll().Where(x => x.LoginName == login.LoginName && x.Password == login.Password).FirstOrDefault();
                if (user != null)
                {
                    HttpContext.Session.SetObject<RegisterUser>("user", user);

                    if (user.TypeId == 1) //admin
                    {
                        return RedirectToAction("index");

                    }
                    else if (user.TypeId == 2) // nv
                    {
                        return RedirectToAction("index", "RegisterProduct");
                    }
                    else if (user.TypeId == 3) // nv giao hàng 
                    {
                        return RedirectToAction("index", "HistoryOrder");
                    }

                }
                else
                {
                    ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
                }

                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login");
        }
        #endregion


        public JsonResult GetAllData(DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var session = HttpContext.Session.GetObject<RegisterUser>("user");
                if (session.Id <= 0)
                {
                    return Json(0, new JsonSerializerOptions());
                }

                dateStart = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
                dateEnd = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);

                //Get danh sách sản phẩm
                var listProducts = (from p in productRepo.GetAll()
                                    join u in unitRepo.GetAll() on p.Unit equals u.Id into t
                                    from u in t.DefaultIfEmpty()
                                    select new
                                    {
                                        Id = p.Id,
                                        p.ProductCode,
                                        p.ProductName,
                                        UnitName = u == null ? "" : t.First().Name,
                                        p.QtyInventory,
                                        p.QtyImport,
                                        p.QtyExport,
                                        p.GiaNhap,
                                        p.GiaBanChung
                                    }).ToList();
                var products = new
                {
                    data = listProducts,
                    totalProduct = listProducts.Count(),
                    totalMoneyImport = listProducts.Sum(x => x.GiaNhap)
                };

                //Get doanh thu
                var listOrders = orderRepo.GetAll().Where(x => x.DeliveryStatus == 1 &&
                                                            (Convert.ToDateTime(x.CreatedDate) >= dateStart && Convert.ToDateTime(x.CreatedDate) <= dateEnd)).ToList();
                var orders = new
                {
                    data = listOrders,
                    totalRevenue = listOrders.Sum(x => x.TongTien)
                };

                //Get data report
                var reports = orderRepo.GetDataReport(dateStart, dateEnd, 0);

                //Get danh sách bán chạy
                var topsales = (from o in orderRepo.GetAll()
                                join d in orderDetailRepo.GetAll() on o.Id equals d.HistoryOrderId into t
                                from d in t.DefaultIfEmpty()
                                join p in productRepo.GetAll() on d.ProductId equals p.Id into t1
                                from p in t1.DefaultIfEmpty()
                                join u in unitRepo.GetAll() on p.Unit equals u.Id into t2
                                from u in t2.DefaultIfEmpty()
                                where o.DeliveryStatus == 1 &&
                                      (Convert.ToDateTime(o.CreatedDate) >= dateStart && Convert.ToDateTime(o.CreatedDate) <= dateEnd)
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
                                    WholesalePrice = p.GiaBanChung,
                                    o.DeliveryStatus
                                }).GroupBy(x => x.Id).Select(x => x.First()).ToList().Take(10);

                var tuple = new Tuple<object, object, object, object>(products, orders, reports, topsales);

                return Json(tuple, new JsonSerializerOptions());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
