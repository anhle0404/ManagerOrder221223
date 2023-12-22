using ManagerOrder.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerOrder.Repo
{
    public class HistoryOrderRepo:GenericRepo<HistoryOrder>
    {
        HistoryOrderDetailRepo detailRepo = new HistoryOrderDetailRepo();
        RegisterProductRepo productRepo = new RegisterProductRepo();
        public object GetDataReport(DateTime dateStart, DateTime dateEnd,int type)
        {
            var listDate = Enumerable.Range(0, 1 + dateEnd.Subtract(dateStart).Days)
                                                .Select(offset => dateStart.AddDays(offset))
                                                .ToList();


            List<double> dataImports = new List<double>();
            List<double> dataRevenues = new List<double>();
            List<double> dataInterests = new List<double>();

            
            foreach (var item in listDate)
            {
                var listOrders = GetAll().Where(x => x.DeliveryStatus > 0 &&
                                                    (Convert.ToDateTime(x.CreatedDate).Year == item.Year &&
                                                    Convert.ToDateTime(x.CreatedDate).Month == item.Month &&
                                                    Convert.ToDateTime(x.CreatedDate).Day == item.Day)).ToList();

                var listOrderDetail = (from o in listOrders
                                       join d in detailRepo.GetAll() on o.Id equals d.HistoryOrderId into t
                                       from d in t.DefaultIfEmpty()
                                       select d).ToList();

                dataImports.Add((double)listOrderDetail.Sum(x => x.GiaNhap));
                dataRevenues.Add((double)listOrders.Sum(x => x.TongTien));

                double totalInterest = (double)listOrders.Sum(x => x.TongTien) - (double)listOrderDetail.Sum(x => x.GiaNhap);
                dataInterests.Add(totalInterest);

            }
            string[] categories = Enumerable.Range(0, 1 + dateEnd.Subtract(dateStart).Days)
                                            .Select(offset => dateStart.AddDays(offset).ToString("dd/MM"))
                                            .ToArray();


            var seriesImport = new
            {
                name = "Tiền nhập",
                data = dataImports.ToArray()
            };

            var seriesRevenue = new
            {
                name = "Doanh thu",
                data = dataRevenues.ToArray()
            };

            var seriesInterest = new
            {
                name = "Tiền lãi",
                data = dataInterests.ToArray()
            };

            var reports = new
            {
                data = new List<object>() { seriesImport, seriesRevenue, seriesInterest },
                categories = categories,
            };

            return reports;
        }
    }
}
