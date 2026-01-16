using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.OrderStatusHistoryServices;
using System.Security.Claims;
using SystemDTOS.OrderStatusHistoryDTOS;
namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusHistoryController : ControllerBase
    {
        private readonly OrderStatusHistoryService _orderStatusHistoryService;
        public OrderStatusHistoryController(OrderStatusHistoryService orderStatusHistoryService)
        {
            _orderStatusHistoryService = orderStatusHistoryService; 
        }
        
        [Authorize(Roles = "Customer,Admin,RestaurantOwner,RestaurantStaff")]
        [HttpGet("OrderHistory/{OrderID}")]
        public ActionResult GetOrderStatusHistoryByID([FromRoute]int OrderID) 
        {
            var userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var history = _orderStatusHistoryService.GetOrderStatusHistoryByOrderID(OrderID, userID);

            return Ok(history);
        }
        
        [Authorize(Roles = "Admin,RestaurantOwner,RestaurantStaff")]
        [HttpGet("ALlRestaurantOrdersHistroy/{RestaurantID}")]
        public ActionResult GetOrdersStatusHistoryBtRestaurantID([FromRoute]int RestaurantID) 
        {
            var userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var history = _orderStatusHistoryService.GetAllOrdersHistory(RestaurantID, userID);

            return Ok(history);
        }
    }
}
