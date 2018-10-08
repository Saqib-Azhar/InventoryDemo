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

namespace DemoInven.Controllers
{
    public class StocksController : Controller
    {
        private DemoInvenEntities db = new DemoInvenEntities();

        // GET: Stocks
        public ActionResult Index()
        {
            var stocks = db.Stocks.Include(s => s.AspNetUser).Include(s => s.Supplier);
            return View(stocks.ToList());
        }

        // GET: Stocks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var stockDetails = db.StockDetails.Where(a=>a.StockId == id);
            if (stockDetails == null)
            {
                return HttpNotFound();
            }
            return View(stockDetails);
        }

        // GET: Stocks/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName");
            return View();
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StockName,StockDescription,CreatedBy,CreatedOn,IsDelete,SupplierId")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                db.Stocks.Add(stock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.AspNetUsers, "Id", "Email", stock.CreatedBy);
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", stock.SupplierId);
            return View(stock);
        }

        // GET: Stocks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.AspNetUsers, "Id", "Email", stock.CreatedBy);
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", stock.SupplierId);
            return View(stock);
        }

        // POST: Stocks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StockName,StockDescription,CreatedBy,CreatedOn,IsDelete,SupplierId")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stock).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.AspNetUsers, "Id", "Email", stock.CreatedBy);
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", stock.SupplierId);
            return View(stock);
        }

        // GET: Stocks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // POST: Stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stock stock = db.Stocks.Find(id);
            db.Stocks.Remove(stock);
            db.SaveChanges();
            return RedirectToAction("Index");
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
        public JsonResult AddStock(Stock StockDetails, List<Product> ProductsDetails)
        {
            try
            {
                var stockObj = new Stock();
                stockObj.StockName = StockDetails.StockName;
                stockObj.StockDescription = StockDetails.StockDescription;
                stockObj.CreatedBy = User.Identity.GetUserId();
                stockObj.CreatedOn = DateTime.Now;
                stockObj.IsDelete = false;
                stockObj.SupplierId = StockDetails.SupplierId;
                db.Stocks.Add(stockObj);
                db.SaveChanges();
                foreach (var item in ProductsDetails)
                {
                    if(item.Id == 0)
                    {
                        Product prodObj = new Product();
                        prodObj.ProductName = item.ProductName;
                        prodObj.IsDelete = false;
                        prodObj.ProductDescription = item.ProductDescription;
                        prodObj.Quantity = item.Quantity;
                        prodObj.StockNo = stockObj.Id;
                        prodObj.Price = item.Price;
                        prodObj.CreatedBy = User.Identity.GetUserId();
                        prodObj.CreatedOn = DateTime.Now;
                        prodObj.BarCode = null;
                        db.Products.Add(prodObj);
                        db.SaveChanges();

                        StockDetail detailObj = new StockDetail();
                        detailObj.ProductId = prodObj.Id;
                        detailObj.QuantityAdded = item.Quantity;
                        detailObj.StockId = stockObj.Id;
                        db.StockDetails.Add(detailObj);
                        db.SaveChanges();
                    }
                    else
                    {
                        Product prodObj = db.Products.FirstOrDefault(d=>d.Id == item.Id);
                        prodObj.ProductName = item.ProductName;
                        prodObj.IsDelete = false;
                        prodObj.ProductDescription = item.ProductDescription;
                        prodObj.Quantity = prodObj.Quantity + item.Quantity;
                        prodObj.StockNo = stockObj.Id;
                        prodObj.Price = item.Price;
                        db.SaveChanges();

                        StockDetail detailObj = new StockDetail();
                        detailObj.ProductId = prodObj.Id;
                        detailObj.QuantityAdded = item.Quantity;
                        detailObj.StockId = stockObj.Id;
                        db.StockDetails.Add(detailObj);
                        db.SaveChanges();
                        

                    }

                }


                return Json("Success");
            }
            catch (Exception)
            {
                return Json("Something went wrong!");
            }
        }
    }
}
