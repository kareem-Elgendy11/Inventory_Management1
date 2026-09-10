using InventoryApp_v2.Data;
using InventoryApp_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Prog1_iti.Controllers
{
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext db;

        public SaleController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // Sales History
        public IActionResult Index()
        {
            var sales = db.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(i => i.Product)
                .OrderByDescending(s => s.SaleDate)
                .ToList();

            return View(sales);
        }

        // Create - GET
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Products = db.Products
                .Where(p => p.Quantity > 0)
                .ToList();

            return View();
        }

        // Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            int ProductId,
            int Quantity,
            string? CustomerInfo)
        {
            var product = db.Products.Find(ProductId);

            if (product == null)
            {
                ModelState.AddModelError("", "Product not found.");
            }
            else if (Quantity <= 0)
            {
                ModelState.AddModelError("Quantity",
                    "Quantity must be greater than zero.");
            }
            else if (Quantity > product.Quantity)
            {
                ModelState.AddModelError("Quantity",
                    $"Not enough stock. Available quantity: {product.Quantity}");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Products = db.Products
                    .Where(p => p.Quantity > 0)
                    .ToList();

                return View();
            }

            // Calculate Total
            decimal total = Quantity * product!.Price;

            // Create Sale
            var sale = new Sale
            {
                SaleDate = DateTime.Now,
                CustomerInfo = CustomerInfo ?? "",
                Totalamount = total
            };

            db.Sales.Add(sale);
            db.SaveChanges();

            // Create SaleItem
            var saleItem = new SaleItem
            {
                SaleId = sale.Id,
                ProductId = product.Id,
                Quantity = Quantity,
                Price = product.Price
            };

            db.SaleItems.Add(saleItem);

            // Decrease Stock
            product.Quantity -= Quantity;

            db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // Details
        public IActionResult Details(int id)
        {
            var sale = db.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(s => s.Id == id);

            if (sale == null)
                return NotFound();

            return View(sale);
        }
    }
}
