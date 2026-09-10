using InventoryApp_v2.Data;
using InventoryApp_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Prog1_iti.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext db;

        public DashboardController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            var vm = new DashboardViewModel
            {
                TotalProducts = db.Products.Count(),
                TotalCategories = db.Categories.Count(),
                TotalSuppliers = db.Suppliers.Count(),
                TotalStockQuantity = db.Products.Sum(p => (int?)p.Quantity) ?? 0,
                LowStockProductsCount = db.Products
                    .Count(p => p.Quantity <= p.LowStokThreshold),

                TotalPurchasesCount = db.Purchases.Count(),
                TotalPurchasesValue = db.Purchases.Sum(p => (decimal?)p.TotalAmount) ?? 0,

                TotalSalesCount = db.Sales.Count(),
                TotalSalesRevenue = db.Sales.Sum(s => (decimal?)s.Totalamount) ?? 0,
            };

            // آخر نشاط - مبيعات
            var recentSales = db.Sales
                .OrderByDescending(s => s.SaleDate)
                .Take(5)
                .Select(s => new ActivityLogItem
                {
                    Type = "Sale",
                    Description = "Sale #" + s.Id,
                    Date = s.SaleDate,
                    Amount = s.Totalamount
                }).ToList();

            // آخر نشاط - مشتريات
            var recentPurchases = db.Purchases
                .OrderByDescending(p => p.PurchaseDate)
                .Take(5)
                .Select(p => new ActivityLogItem
                {
                    Type = "Purchase",
                    Description = "Purchase #" + p.Id,
                    Date = p.PurchaseDate,
                    Amount = p.TotalAmount
                }).ToList();

            vm.RecentActivity = recentSales.Concat(recentPurchases)
                .OrderByDescending(a => a.Date)
                .Take(10)
                .ToList();

            // الأكتر مبيعًا
            vm.MostSoldProducts = db.SaleItems
                .Include(si => si.Product)
                .GroupBy(si => si.ProductId)
                .Select(g => new TopProductItem
                {
                    ProductName = g.First().Product.Name,
                    TotalQuantitySold = g.Sum(si => si.Quantity),
                    TotalRevenue = g.Sum(si => si.Quantity * si.Price)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(5)
                .ToList();

            return View(vm);
        }
    }
}