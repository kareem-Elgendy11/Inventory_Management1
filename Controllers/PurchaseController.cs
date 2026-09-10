using InventoryApp_v2.Data;
using InventoryApp_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp_v2.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext db;

        public PurchaseController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // Purchase History
        public IActionResult Index()
        {
            var purchases = db.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .OrderByDescending(p => p.PurchaseDate)
                .ToList();

            return View(purchases);
        }

        // Create - GET
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Suppliers = db.Suppliers.ToList();
            ViewBag.Products = db.Products.ToList();

            return View();
        }

        // Create - POST
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int SupplierId, int ProductId, int Quantity)
        {
            if (Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Quantity must be greater than zero.");
            }

            var product = db.Products.Find(ProductId);
            if (product == null)
            {
                ModelState.AddModelError("ProductId", "Product not found.");
            }

            var supplier = db.Suppliers.Find(SupplierId);
            if (supplier == null)
            {
                ModelState.AddModelError("SupplierId", "Supplier not found.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers = db.Suppliers.ToList();
                ViewBag.Products = db.Products.ToList();
                return View();
            }

            decimal totalAmount = Quantity * product!.Price;

            // 1. إنشاء فاتورة الشراء (Header)
            var purchase = new Purchase
            {
                PurchaseDate = DateTime.Now,
                SupplierId = SupplierId,
                TotalAmount = totalAmount
            };

            db.Purchases.Add(purchase);
            db.SaveChanges(); 

            // 2. 
            var item = new PurchaseItem
            {
                PurchaseId = purchase.Id,
                ProductId = ProductId,
                Quantity = Quantity,
                UnitPrice = product.Price
            };

            db.PurchaseItems.Add(item);

            // 3. زيادة المخزون تلقائيًا
            product.Quantity += Quantity;

            db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // Details
        public IActionResult Details(int id)
        {
            var purchase = db.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .FirstOrDefault(p => p.Id == id);

            if (purchase == null)
                return NotFound();

            return View(purchase);
        }
    }
}