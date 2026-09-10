using InventoryApp_v2.Data;
using InventoryApp_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using Prog1_iti.viiewmodels.supplierviewmodel;

namespace Prog1_iti.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext db;
        public SupplierController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            var supplier = db.Suppliers.ToList();
            if (supplier == null)
            {

            }
            IndexSuviewmpdel model = new IndexSuviewmpdel()
            {
                suppliers = supplier.Select(s => new supplierviewmodel()
                {
                    IdS = s.Id,
                    ContactNameS = s.ContactName,
                    PhoneS = s.Phone,
                    EmailS = s.Email,
                    AddressS = s.Address,
                    SupplierNameS = s.SupplierName


                }).ToList()

            };
            return View("Index", model);
        }
        [HttpGet]
        public IActionResult add()
        {
            return View("add");
        }
        [HttpPost]
        public IActionResult add(supplierviewmodel model)
        {
            if (!ModelState.IsValid)
            {
                return View("add", model);
            }
            Supplier s = new Supplier()
            {
                SupplierName = model.SupplierNameS,
                ContactName = model.ContactNameS,
                Phone = model.PhoneS,
                Email = model.EmailS,
                Address = model.AddressS
            };
            db.Suppliers.Add(s);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult details(int id)
        {
            var supplier = db.Suppliers
                .Include(c => c.Purchases)
                    .ThenInclude(p => p.PurchaseItems)
                        .ThenInclude(pi => pi.Product)
                            .ThenInclude(prod => prod.Category)
                .FirstOrDefault(c => c.Id == id);

            if (supplier == null || supplier.Purchases == null)
            {
                return RedirectToAction("Index");
            }
            supplierviewmodel model = new supplierviewmodel()
            {
                IdS = supplier.Id,
                ContactNameS = supplier.ContactName,
                SupplierNameS = supplier.SupplierName,
                PhoneS = supplier.Phone,
                EmailS = supplier.Email,
                AddressS = supplier.Address,
                purchases = supplier.Purchases
                //products=supplier.product

            };
            return View("details", model);
        }
        [HttpGet]
        public IActionResult edit(int id)
        {
            var supplier = db.Suppliers.Where(p => p.Id == id).FirstOrDefault();
            if (supplier == null)
            {
                return RedirectToAction("Index");
            }
            supplierviewmodel model = new supplierviewmodel()
            {
                IdS = supplier.Id,
                ContactNameS = supplier.ContactName,
                SupplierNameS = supplier.SupplierName,
                PhoneS = supplier.Phone,
                EmailS = supplier.Email,
                AddressS = supplier.Address
            };

            return View("edit", model);
        }
        [HttpPost]
        public IActionResult edit(supplierviewmodel model)
        {
            if (!ModelState.IsValid)
            {
                return View("edit", model);
            }
            var supplier = db.Suppliers.Where(s => s.Id == model.IdS).FirstOrDefault();
            supplier.SupplierName = model.SupplierNameS;
            supplier.ContactName = model.ContactNameS;
            supplier.Phone = model.PhoneS;
            supplier.Email = model.EmailS;
            supplier.Address = model.AddressS;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var supplier = db.Suppliers.FirstOrDefault(s => s.Id == id);
            if (supplier == null)
            {
                return RedirectToAction("Index");
            }
            var relatepurchaseitem = db.Purchases.Where(p => p.SupplierId == id).ToList();
            if (relatepurchaseitem.Any())
            {
                var purchesid = relatepurchaseitem.Select(p => p.Id).ToList();
                var relateitem = db.PurchaseItems.Where(pi => purchesid.Contains(pi.PurchaseId)).ToList();
                if (relateitem.Any())
                {
                    db.Purchases.RemoveRange(relatepurchaseitem);

                }

            }
            db.Suppliers.Remove(supplier);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}