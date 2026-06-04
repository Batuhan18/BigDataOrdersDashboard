using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly BigDataOrdersDbContext _context;

        public StatisticsController(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.CategoryCount = _context.Categories.Count();
            ViewBag.CustomerCount = _context.Customers.Count();
            ViewBag.ProductCount = _context.Products.Count();
            ViewBag.OrderCount = _context.Orders.Count();

            ViewBag.CustomerCountry = _context.Customers.Select(x=>x.CustomerCountry).Distinct().Count();
            ViewBag.CustomerCity = _context.Customers.Select(x=>x.CustomerCity).Distinct().Count();
            ViewBag.OrderStatusByCompleted = _context.Orders.Where(x=>x.OrderStatus=="Tamamlandı").Distinct().Count();
            ViewBag.OrderStatusByCancelled = _context.Orders.Where(x=>x.OrderStatus=="İptal Edildi").Distinct().Count();

            ViewBag.OctoberOrders = _context.Orders.FromSqlRaw("SELECT * FROM Orders WHERE OrderDate>='2025-10-01' AND OrderDate<'2025-11-01'").Count();
            ViewBag.Orders2025Count = _context.Orders.Where(x=>x.OrderDate.Year==2025).Count();
            ViewBag.AverageProductPrice = _context.Products.Average(x => x.UnitPrice);
            ViewBag.AverageProductQuantity = _context.Products.Average(x => x.StockQuantity);
            return View();
        }

        public IActionResult TextualStatistics()
        {
            ViewBag.MostExpensiveProduct = _context.Products.Where(x => x.UnitPrice == (_context.Products.Max(x => x.UnitPrice))).Select(y => y.ProductName).FirstOrDefault();
            ViewBag.CheapestProduct = _context.Products.Where(x => x.UnitPrice == (_context.Products.Min(x => x.UnitPrice))).Select(y => y.ProductName).FirstOrDefault();
            return View();
        }
    }
}
