using InventoryApp_v2.Data;
using InventoryApp_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Prog1_iti.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext db;
        public ProductController(ApplicationDbContext db)
        {
            this.db = db;
        }
        //display all product
        public IActionResult Browse(int page=1,string word="")
        {
            int pagesize = 10;
            int countpage =(int)Math.Ceiling((decimal)db.Products.Count()/10);
            //decimal countpage=db.Products.Count();
            List<Product> products = null;
            if (page<=0)
            {
                page = 1;
            }
            if (countpage > 0 && page > countpage)
            {
                page = countpage;
            }
            if (!string.IsNullOrEmpty(word))
            {
                products = db.Products.Include(p => p.Category).Where(p => p.Name.Contains(word) ||p.Category.Name.Contains(word)).Skip((page - 1) * pagesize).Take(pagesize).ToList();

            }
            else
            {
                products = db.Products.Include(p => p.Category).Skip((page - 1) * pagesize).Take(pagesize).ToList();

            }
            Indexviewmodel model=new Indexviewmodel()
            {
                products = products,
                currentpage = page,
                countpage = countpage,
            };
            return View("Browse",model);
        }
        [HttpGet]
        public IActionResult add()
        {
            //var product = db.Products.Where(p => p.Id == id).FirstOrDefault();
            var category = db.Categories.ToList();
            productviewmodel model = new productviewmodel()
            {
                Categories = category
            };
            return View("add",model);
        }
        [HttpPost]
        public IActionResult add(productviewmodel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = db.Categories.ToList();
                return View ("add",model);
            }
            Product product=new Product()
            {
                Name=model.Name,
                Price=model.Price,
                Quantity=model.Quantity,
                CategoryId=model.CategoryId,
                LowStokThreshold=model.LowStokThreshold,
                sku=model.sku
                
            };
            db.Products.Add(product);
            db.SaveChanges();
            return RedirectToAction("Browse");
        }
        [HttpGet]
        public IActionResult edit(int id)
        {
            var product = db.Products.Where(p => p.Id == id).FirstOrDefault();
            if(product==null)
            {
                return RedirectToAction("Browse");
            }
            var category=db.Categories.ToList();
            productviewmodel model = new productviewmodel()
            {
                Id=product.Id,
                Name=product.Name,
                Price= product.Price,
                Quantity=product.Quantity,
                CategoryId=product.CategoryId,
                Categories=category,
                LowStokThreshold = product.LowStokThreshold,
                sku = product.sku
            };

            return View("edit",model);
        }
        [HttpPost]
        public IActionResult edit(productviewmodel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = db.Categories.ToList();
                return View("edit",model);
            }
           var product=db.Products.Where(p=>p.Id==model.Id).FirstOrDefault();
            product.Name = model.Name;
            product.Price = model.Price;
            product.Quantity = model.Quantity;
            product.CategoryId = model.CategoryId;
            product.LowStokThreshold = model.LowStokThreshold;
            product.sku = model.sku;
            db.SaveChanges();
            return RedirectToAction("Browse");
        }
        public IActionResult delete(int id)
        {
            var product=db.Products.Where(p=>p.Id==id).FirstOrDefault();
            if(product==null)
            {
                return RedirectToAction("Browse");
            }
            var relatepurchaseitem = db.PurchaseItems.Where(p => p.ProductId == id).ToList();
            if (relatepurchaseitem.Any())
            {
                db.PurchaseItems.RemoveRange(relatepurchaseitem);
                
            }
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Browse");
           

        }
        public IActionResult Search(string word)
        {
            var products = db.Products.Where(p => p.Name == word ||p.Category.Name==word);
            return View();
        }
        public IActionResult details(int id)
        {
            var product = db.Products.Where(p => p.Id == id).FirstOrDefault();
            if (product == null)
            {
                return RedirectToAction("Browse");
            }
            var category = db.Categories.Where(c => c.Id == product.CategoryId).FirstOrDefault();

            productviewmodel model = new productviewmodel()
            {
                Id=product.Id,
                Name=product.Name,
                Price=product.Price,
                Quantity=product.Quantity,
                CategoryId=product.CategoryId,
                LowStokThreshold=product.LowStokThreshold,
                sku=product.sku,
                //Categories=category
                categoryname=category.Name
                
            };
            return View("details", model);
        }
    }
}
