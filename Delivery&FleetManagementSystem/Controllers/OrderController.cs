using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using ServiceLayer.OrderServices;
using System.Security.Claims;
using SystemContext.SystemDbContext;
using SystemDTOS.OrderDTOS;
using SystemModel.Entities;

namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public readonly OrderService _orderService;
        public OrderController(OrderService orderService)
        {
            _orderService = orderService;

        }
        #region Create
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<ActionResult> CreateOrder([FromBody]CreateOrderDTO dto) 
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      
            
            var order = await _orderService.CreateOrder(dto,int.Parse(userID));
            return Ok(order);

        }
        #endregion

        #region Accept
        [Authorize(Roles = "RestaurantOwner,RestaurantStaff")]
        [HttpPost("acceptorder/{orderID}")]
        public async Task<ActionResult> AcceptOrder([FromRoute]int orderID)
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            await _orderService.AcceptOrderAsync(orderID, int.Parse(userID));
            return Ok(new
            {
             message = "Order Accepted",
             OrderID = orderID  
            });
        }
        #endregion

        #region AssignDriver
        [Authorize(Roles = "RestaurantOwner,RestaurantStaff")]
        [HttpPost("assigndriver")]
        public async Task<ActionResult> AssignDriver([FromQuery] int orderID)
        {
            var UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _orderService.AssignDriverAsync(orderID,UserID);
            return Ok(new
            {
                message = "Driver Assigned",
                OrderID = orderID
               });
        }
        #endregion

        #region Pickup
        [Authorize(Roles = "Driver")]
        [HttpPost("pickuporder/{orderID}")]
        public async Task<ActionResult> PickupOrder([FromRoute] int orderID) 
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
         
           await _orderService.PickupOrderAsync(orderID, int.Parse(userID));
            return Ok(new
            {
                message = "Order Pickedup",
                OrderID = orderID
            });
        }
        #endregion

        #region Deliver
        [Authorize(Roles = "Driver")]
        [HttpPost("deliverorder/{orderID}")]
        public async Task<ActionResult> DeliverOrder([FromRoute] int orderID)
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
         

            await _orderService.DeliverOrderAsync(orderID, int.Parse(userID));
            return Ok(new
            {
                message = "Order Delivered",
                OrderID = orderID
            });
        }
        #endregion
        
        #region Cancel
        [Authorize(Roles = "Customer,RestaurantOwner,RestaurantStaff")]
        [HttpPost("cancelorder")]
        public async Task<ActionResult> CancelOrder([FromQuery] int orderID,[FromQuery] string reason) 
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _orderService.CancelOrderAsync(orderID, reason , int.Parse(userID));
            return Ok(new
            {
                message = "Order Canceled",
                OrderID = orderID
            });
        }
        #endregion

        #region Get My Orders

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public ActionResult GetMyOrders()
        {
            var userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var Orders = _orderService.GetMyOrders(userID);
            return Ok(Orders);
        }
        #endregion


    }
}
