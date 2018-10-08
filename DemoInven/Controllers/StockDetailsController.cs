using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DemoInven.Models;

namespace DemoInven.Controllers
{
    public class StockDetailsController : Controller
    {
        private DemoInvenEntities db = new DemoInvenEntities();

        // GET: StockDetails
        public ActionResult Index()
        {
            var stockDetails = db.StockDetails.Include(s => s.Product).Include(s => s.Stock);
            return View(stockDetails.ToList());
        }

        // GET: StockDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockDetail stockDetail = db.StockDetails.Find(id);
            if (stockDetail == null)
            {
                return HttpNotFound();
            }
            return View(stockDetail);
        }

        // GET: StockDetails/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Products, "Id", "ProductDescription");
            ViewBag.StockId = new SelectList(db.Stocks, "Id", "StockName");
            return View();
        }

        // POST: StockDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StockId,ProductId,QuantityAdded")] StockDetail stockDetail)
        {
            if (ModelState.IsValid)
            {
                db.StockDetails.Add(stockDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.Products, "Id", "ProductDescription", stockDetail.ProductId);
            ViewBag.StockId = new SelectList(db.Stocks, "Id", "StockName", stockDetail.StockId);
            return View(stockDetail);
        }

        // GET: StockDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockDetail stockDetail = db.StockDetails.Find(id);
            if (stockDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "ProductDescription", stockDetail.ProductId);
            ViewBag.StockId = new SelectList(db.Stocks, "Id", "StockName", stockDetail.StockId);
            return View(stockDetail);
        }

        // POST: StockDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StockId,ProductId,QuantityAdded")] StockDetail stockDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "ProductDescription", stockDetail.ProductId);
            ViewBag.StockId = new SelectList(db.Stocks, "Id", "StockName", stockDetail.StockId);
            return View(stockDetail);
        }

        // GET: StockDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockDetail stockDetail = db.StockDetails.Find(id);
            if (stockDetail == null)
            {
                return HttpNotFound();
            }
            return View(stockDetail);
        }

        // POST: StockDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StockDetail stockDetail = db.StockDetails.Find(id);
            db.StockDetails.Remove(stockDetail);
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
    }
}
