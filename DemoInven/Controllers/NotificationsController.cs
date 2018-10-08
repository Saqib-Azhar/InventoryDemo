using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoInven.Models;

namespace DemoInven.Controllers
{
    public class NotificationsController : Controller
    {
        [HttpPost]
        public JsonResult GetNotifications()
        {
            var db = new DemoInvenEntities();
            var notifications = db.Notifications.Where(s => s.StatusId == 1);

            return Json(notifications, JsonRequestBehavior.AllowGet);
        }
    }
}