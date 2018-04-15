using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.Orders;

namespace WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Orders")]
    public class OrdersController : Controller
    {
        [HttpGet("/api/Orders/all/{userId}", Name = "GetAllOrdersFromUser", Order = 1)]
        public List<OrderViewModel> GetAllOrders(int userId)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            return _context.GetCurrentOrdersGeneral(userId);
        }

        [HttpGet("/api/Orders/{orderId}", Name = "GetOrderItems", Order = 1)]
        public List<OrderComponent> GetOrderItems(int orderId)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            return _context.GetOrderItems(orderId);
        }
    }
}