using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoInven.Models
{
    public class OrdersTupple
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}