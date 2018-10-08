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
    [Authorize]
    public class OrdersController : Controller
    {
        private DemoInvenEntities db = new DemoInvenEntities();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.AspNetUser);
            return View(orders.ToList());
        }
        [Authorize]

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orderDetails = db.OrderDetails.Where(s=>s.OrderId == id);
            if (orderDetails == null)
            {
                return HttpNotFound();
            }
            return View("","",orderDetails);
        }
        [Authorize]
        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.CashierId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.Products = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public JsonResult SubmitOrder(string CustomerName, int TotalPrice, List<OrderDetail> OrderDetails)
        {
            try
            {
                var Order = new Order();
                Order.BuyerName = CustomerName;
                Order.BoughtAt = DateTime.Now;
                Order.CashierId = User.Identity.GetUserId();
                Order.IsDeleted = false;
                Order.TotalPrice = TotalPrice;
                db.Orders.Add(Order);

                db.SaveChanges();
                foreach (var item in OrderDetails)
                {
                    OrderDetail detail = new OrderDetail();
                    detail.OrderId = Order.Id;
                    detail.ProductId = item.ProductId;
                    detail.Quantity = item.Quantity;
                    detail.Price = item.Price;

                    var thisProd = db.Products.FirstOrDefault(s => s.Id == item.ProductId);
                    thisProd.Quantity = thisProd.Quantity - item.Quantity;
                    
                    if(thisProd.Quantity < 10)
                    {
                        var newNotification = new Notification();
                        newNotification.GeneratedOn = DateTime.Now;
                        newNotification.LastUpdated = DateTime.Now;
                        newNotification.NotificationDetail = "Only " + thisProd.Quantity + " is remaining in Stock of Product " + thisProd.ProductName;
                        newNotification.StatusId = 1;
                        db.Notifications.Add(newNotification);
                    }

                    db.OrderDetails.Add(detail);
                    db.SaveChanges();
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Something went wrong", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
