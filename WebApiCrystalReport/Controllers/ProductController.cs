using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Data.Entity;
using WebApiCrystalReport.Models;
using WebApiCrystalReport.ViewModels;

namespace WebApiCrystalReport.Controllers
{
    public class ProductController : Controller
    {
        public ActionResult Index()
        {
            //var model = new ProductViewModel();
            //using (var context=new CoreCrudEntities())
            //{
            //    model.allproducts = context.Products.Include(p => p.Category).ToList();
            //}
            return View();
        }
        public JsonResult GetProductList(string sidx, string sort, int page, int rows)
        {
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var context = new CoreCrudEntities();
            var dbdata= context.Products.Include(p => p.Category).ToList();

            var products = (from product in dbdata
                            select new
                            {
                                Id=product.Id,
                                Name= product.Name,
                                CategoryId = product.Category.Name,
                                Description=product.Description,
                                 
                             });
            int totalRecords = products.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sort.ToUpper() == "DESC")
            {
                products = products.OrderByDescending(t => t.Name);
                products = products.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                products = products.OrderBy(t => t.Name);
                products = products.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = products
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoryList()
        {
            var context = new CoreCrudEntities();
            context.Configuration.ProxyCreationEnabled = false;
           
            var categoryList = context.Categories.ToList();
            var category = (from cat in categoryList
                            select new
                            {
                                Id = cat.Id,
                                Value = cat.Name,

                            }).ToList();
            //List<SelectListItem> items = new List<SelectListItem>();
            //foreach(var cat in categoryList)
            //{
            //    items.Add(new SelectListItem
            //    {
            //        Text = cat.Name,
            //        Value = cat.Id.ToString()
            //    });
            //}


            return Json(category, JsonRequestBehavior.AllowGet);
        }

        public string Create(Product product)
        {

            var context = new CoreCrudEntities();

            context.Products.Add(product);
            var status = context.SaveChanges();

            return status > 0 ? "Create Successfully" : "error";
        }
        public string Edit(Product product)
        {
           
                var context = new CoreCrudEntities();
                context.Entry(product).State = EntityState.Modified;
            var status =context.SaveChanges();

            return status > 0 ? "Edit Successfully" : "error";
        }
        public string Delete(int Id)
        {

            var context = new CoreCrudEntities();
            var data = context.Products.Find(Id);
            context.Entry(data).State = EntityState.Deleted;
            var status = context.SaveChanges();

            return status > 0 ? "Delete Successfully" : "error";
        }
    }
}
