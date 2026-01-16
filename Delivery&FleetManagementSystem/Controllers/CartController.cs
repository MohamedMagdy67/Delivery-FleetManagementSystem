using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.OrderServices;
using SystemDTOS.OrderDTOS;

namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;
        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }
        
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public ActionResult CreateCart([FromBody]List<CartDTO> dto)
        {
            var Cart = _cartService.CreateCart(dto);
           
            return Ok(Cart);
        }

    }
}
