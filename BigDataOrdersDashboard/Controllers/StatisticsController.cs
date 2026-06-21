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

            ViewBag.CustomerCountry = _context.Customers.Select(x => x.CustomerCountry).Distinct().Count();
            ViewBag.CustomerCity = _context.Customers.Select(x => x.CustomerCity).Distinct().Count();
            ViewBag.OrderStatusByCompleted = _context.Orders.Where(x => x.OrderStatus == "Tamamlandı").Distinct().Count();
            ViewBag.OrderStatusByCancelled = _context.Orders.Where(x => x.OrderStatus == "İptal Edildi").Distinct().Count();

            ViewBag.OctoberOrders = _context.Orders.FromSqlRaw("SELECT * FROM Orders WHERE OrderDate>='2025-10-01' AND OrderDate<'2025-11-01'").Count();
            ViewBag.Orders2025Count = _context.Orders.Where(x => x.OrderDate.Year == 2025).Count();
            ViewBag.AverageProductPrice = _context.Products.Average(x => x.UnitPrice);
            ViewBag.AverageProductQuantity = _context.Products.Average(x => x.StockQuantity);
            return View();
        }

        public IActionResult TextualStatistics()
        {
            ViewBag.MostExpensiveProduct = _context.Products.Where(x => x.UnitPrice == (_context.Products.Max(x => x.UnitPrice))).Select(y => y.ProductName).FirstOrDefault();
            ViewBag.CheapestProduct = _context.Products.Where(x => x.UnitPrice == (_context.Products.Min(x => x.UnitPrice))).Select(y => y.ProductName).FirstOrDefault();
            ViewBag.TopStockProduct = _context.Products.OrderByDescending(x => x.StockQuantity).Take(1).Select(y => y.ProductName).FirstOrDefault();
            ViewBag.LastAddedProduct = _context.Products.OrderByDescending(x => x.ProductId).Take(1).Select(y => y.ProductName).FirstOrDefault();
            ViewBag.LastAddedCustomer = _context.Customers.OrderByDescending(x => x.CustomerId).Take(1).Select(y => y.CustomerName + " " + y.CustomerSurname).FirstOrDefault();

            ViewBag.TopPaymentMethod = _context.Orders.GroupBy(x => x.PaymentMethod).Select(y => new
            {
                PaymentMethod = y.Key,
                TotalOrders = y.Count()
            }).OrderByDescending(k => k.TotalOrders).Select(o => o.PaymentMethod).FirstOrDefault();

            ViewBag.TopOrderedProduct = _context.Orders.GroupBy(o => o.Product.ProductName).Select(y => new
            {
                ProductName = y.Key,
                TotalQuantity = y.Sum(o => o.Quantity)
            }).OrderByDescending(x => x.TotalQuantity).Select(z => z.ProductName).FirstOrDefault();

            ViewBag.MinOrderedProduct = _context.Orders.GroupBy(o => o.Product.ProductName).Select(y => new
            {
                ProductName = y.Key,
                TotalQuantity = y.Sum(o => o.Quantity)
            }).OrderBy(x => x.TotalQuantity).Select(z => z.ProductName).FirstOrDefault();

            ViewBag.TopCountry = _context.Orders.GroupBy(x => x.Customer.CustomerCountry).Select(k => new
            {
                CustomerCountry = k.Key,
                TotalQuantity = k.Count()
            }).OrderByDescending(p => p.TotalQuantity).Select(c => c.CustomerCountry).FirstOrDefault();

            ViewBag.TopCity = _context.Orders.GroupBy(x => x.Customer.CustomerCity).Select(k => new
            {
                CustomerCity = k.Key,
                TotalQuantity = k.Count()
            }).OrderByDescending(p => p.TotalQuantity).Select(c => c.CustomerCity).FirstOrDefault();

            ViewBag.TopCategory = _context.Orders.GroupBy(o => o.Product.Category.CategoryName).Select(g => new
            {
                CategoryName = g.Key,
                TotalSales = g.Sum(x => x.Quantity)
            }).OrderByDescending(x => x.TotalSales).Select(x => x.CategoryName).FirstOrDefault();

            ViewBag.TopCustomer = _context.Orders.GroupBy(o => new { o.Customer.CustomerName, o.Customer.CustomerSurname }).Select(g => new
            {
                FullName = g.Key.CustomerName + " " + g.Key.CustomerSurname,
                TotalOrders = g.Count()
            }).OrderByDescending(x => x.TotalOrders).Select(x => x.FullName).FirstOrDefault();

            ViewBag.MostCompletedProduct = _context.Orders.Where(o => o.OrderStatus == "Tamamlandı").GroupBy(o => o.Product.ProductName).Select(g => new
            {
                ProductName = g.Key,
                CompletedOrders = g.Count()
            }).OrderByDescending(x => x.CompletedOrders).Select(x => x.ProductName).FirstOrDefault();

            ViewBag.TopReturnedProduct = _context.Orders.Where(o => o.OrderStatus == "İptal Edildi").GroupBy(o => o.Product.ProductName).Select(g => new
            {
                ProductName = g.Key,
                CompletedOrders = g.Count()
            }).OrderByDescending(x => x.CompletedOrders).Select(x => x.ProductName).FirstOrDefault();

            ViewBag.LowestStockProduct = _context.Products.OrderBy(x => x.StockQuantity).Take(1).Select(y => y.ProductName).FirstOrDefault();

            ViewBag.LowestActiveCategory = _context.Orders.GroupBy(o => o.Product.Category.CategoryName).Select(g => new
            {
                CategoryName = g.Key,
                TotalSales = g.Sum(x => x.Quantity)
            }).OrderBy(x => x.TotalSales).Select(x => x.CategoryName).FirstOrDefault();



            return View();
        }
    }
}
