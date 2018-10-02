using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DemoInven.Models;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;

namespace DemoInven.Controllers
{
    public class ProductsController : Controller
    {
        private DemoInvenEntities db = new DemoInvenEntities();
        static public List<Product> ProductsStaticList = new List<Product>();
        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.AspNetUser).Include(p => p.Category);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [Authorize(Roles = "Admin")]

        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductName,CategoryId,CreatedOn,CreatedBy,IsDelete,BarCode,Quantity,StockNo,ProductDescription")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedBy = User.Identity.GetUserId();
                product.CreatedOn = DateTime.Now;
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.AspNetUsers, "Id", "Email", product.CreatedBy);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }

        [Authorize(Roles = "Admin")]

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.AspNetUsers, "Id", "Email", product.CreatedBy);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductName,CategoryId,CreatedOn,CreatedBy,IsDelete,BarCode,Quantity,StockNo,ProductDescription")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.AspNetUsers, "Id", "Email", product.CreatedBy);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }

        [Authorize(Roles = "Admin")]

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public bool DecreaseQuantity(int id, int quantity)
        {
            try
            {
                var product = db.Products.Find(id);
                product.Quantity = product.Quantity - quantity;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        [HttpPost]
        public JsonResult SearchProduct(string query)
        {
            try
            {
                int n;
                bool isNumeric = int.TryParse(query, out n);
                if (ProductsStaticList.Count == 0 || ProductsStaticList == null)
                {
                    ProductsStaticList = db.Products.ToList();
                }

                if (isNumeric == true)
                {
                    var integerValue = Convert.ToInt32(query);
                    var results = (from obj in ProductsStaticList where obj.Id == integerValue select new { Id = obj.Id, Name = obj.Id + "-" + obj.ProductName }).Take(25).ToList();
                    return Json(results, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    var newQuery = query.First().ToString().ToUpper() + query.Substring(1);
                    var results = (from obj in ProductsStaticList where (obj.ProductName.Contains(query) || obj.ProductName.Contains(newQuery)) select new { Id = obj.Id, Name = obj.Id + "-" + obj.ProductName }).Take(25).ToList();
                    return Json(results, JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {

                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetPrice(int Id)
        {
            try
            {
                if (ProductsStaticList.Count == 0 || ProductsStaticList == null)
                {
                    ProductsStaticList = db.Products.ToList();
                }
                var prod = ProductsStaticList.Find(s=>s.Id == Id);
                return Json(prod.Price, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }


    }
}
