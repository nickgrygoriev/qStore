using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qStore.Models;
using qStore.Filters;

namespace qStore.Controllers
{
    public class AdminController : Controller
    {
        [SimpleAuthorize(Users="admin@gmail.com")] //pass:11111
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetCategories()
        {
            List<Category> categories;

            using (var db = new qStoreDBEntities())
            {
                categories = db.Categories.ToList();
            }

            return PartialView(categories);
        }

        public ActionResult GetProducts()
        {
            List<Product> prods;

            using (var db = new qStoreDBEntities())
            {
                prods = db.Products.ToList();
            }

            return PartialView(prods);
        }

        [HttpGet]
        public ActionResult GetProductDetails(Guid id)
        {
            Product prod;

            using (var db = new qStoreDBEntities())
            {
                prod = db.Products.SingleOrDefault(x => x.Id == id);
            }

            return PartialView(prod);
        }

        public ActionResult EditProduct(Guid? id)
        {
            Product prod = null;

            if (id.HasValue)
            {
                using (var db = new qStoreDBEntities())
                {
                    prod = db.Products.SingleOrDefault(x => x.Id == id);
                }
            }

            if (prod == null)
            {
                prod = new Product
                {
                    Id = Guid.NewGuid()
                };
            }

            using (var db = new qStoreDBEntities())
            {
                var catList = db.Categories.Where(x => !x.IsDisabled).ToList()
                    .Select(x => new SelectListItem { Text = x.Title, Value = x.Id.ToString() }).ToList();

                ViewBag.Categories = catList;
            }

            return View(prod);
        }

        public ActionResult EditCategory(Guid? id)
        {
            Category cat = null;

            if (id.HasValue)
            {
                using (var db = new qStoreDBEntities())
                {
                    cat = db.Categories.SingleOrDefault(x => x.Id == id);
                }
            }

            if (cat == null)
            {
                cat = new Category
                      {
                          Id = Guid.NewGuid()
                      };
            }

            return View(cat);
        }

        public ActionResult DeleteProduct(Guid id)
        {
            using (var db = new qStoreDBEntities())
            {
                var prod = db.Products.SingleOrDefault(x => x.Id == id);

                if (prod != null)
                {
                    db.Products.Remove(prod);
                }
                else
                {
                    throw new InvalidOperationException("Product not found");
                }

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult DeleteCategory(Guid id)
        {
            using (var db = new qStoreDBEntities())
            {
                var cat = db.Categories.SingleOrDefault(x => x.Id == id);

                if (cat != null)
                {
                    db.Categories.Remove(cat);
                }
                else
                {
                    throw new InvalidOperationException("Category not found");
                }

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SaveProduct(HttpPostedFileBase file, Product model)
        {
            using (var db = new qStoreDBEntities())
            {
                var product = db.Products.SingleOrDefault(x => x.Id == model.Id);
                model.Category = db.Categories.SingleOrDefault(x => x.Id == model.CategoryId);

                if (file != null && file.ContentLength > 0)
                {
                    model.ImageContentType = file.ContentType;
                    model.ImageBase64String = GetByteArrayString(file);
                }
                
                if (product != null)
                {
                    product.Title = model.Title;
                    product.Description = model.Description;
                    product.Category = model.Category;
                    product.ImageBase64String = model.ImageBase64String;
                    product.ImageContentType = model.ImageContentType;
                    product.Price = model.Price;
                }
                else
                {
                    db.Products.Add(model);
                }

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SaveCategory(Category model)
        {
            using (var db = new qStoreDBEntities())
            {
                var cat = db.Categories.SingleOrDefault(x => x.Id == model.Id);

                if (cat != null)
                {
                    cat.Title = model.Title;
                    cat.IsDisabled = model.IsDisabled;
                }
                else
                {
                    db.Categories.Add(model);
                }

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private string GetByteArrayString(HttpPostedFileBase file)
        {
            byte[] data = new byte[] { };

            using (var binaryReader = new BinaryReader(file.InputStream))
            {
                data = binaryReader.ReadBytes(file.ContentLength);
            }

            return Convert.ToBase64String(data);
        }
    }
}