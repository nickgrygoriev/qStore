using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qStore.Models;

namespace qStore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CategoryList()
        {
            List<Category> categories;

            using (var db = new qStoreDBEntities())
            {
                categories = db.Categories.ToList();
            }

            return View(categories);
        }

        public ActionResult ProductList(Guid? id)
        {
            List<ProductInfo> prods;

            using (var db = new qStoreDBEntities())
            {
                Category category = null;

                if (id.HasValue)
                {
                    category = db.Categories.SingleOrDefault(x => x.Id == id);
                }

                if (category == null)
                {
                    category = db.Categories.First(x => !x.IsDisabled);
                }

                prods = category.Products
                    .Select(x => new ProductInfo
                            {
                                Description = x.Description,
                                Id = x.Id,
                                ImageBase64String = x.ImageBase64String,
                                ImageContentType = x.ImageBase64String,
                                Price = x.Price,
                                Title = x.Title
                            }).ToList();
            }

            return View(prods);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}