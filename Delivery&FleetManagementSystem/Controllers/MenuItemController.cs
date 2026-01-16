using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.MenuItemServices;
using System.Security.Claims;
using SystemDTOS.MenuItemDTOS;

namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly MenuItemService _menuItemService;
        public MenuItemController(MenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        [Authorize(Roles = "Admin,RestaurantOwner,RestaurantStaff")]
        [HttpPost]
        public ActionResult CreateMenuItem([FromBody]CreateMenuItemDTO dto)
        {
            var UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var NewItem = _menuItemService.CreateMenuItem(dto,UserID);
            return Ok(NewItem);
        }

        [Authorize(Roles = "Admin,RestaurantOwner,RestaurantStaff")]
        [HttpPut("{ItemID}")]
        public ActionResult UpdateMenuItem([FromRoute]int ItemID,[FromBody] UpdateMenuItemDTO dto)
        {
            var UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var UpdatedItem = _menuItemService.UpdateMenuItem(ItemID,dto, UserID);
            return Ok(UpdatedItem);
        }

        [Authorize]
        [HttpGet("MenuItems/{RestaurantID}")]
        public ActionResult GetMenuItemsByRestaurantID([FromRoute] int RestaurantID)
        {

            var MenuItems = _menuItemService.GetMenuItemsByRestaurantID(RestaurantID);

            return Ok(MenuItems);
        }

        [Authorize]
        [HttpGet("Item/{ItemID}")]
        public ActionResult GetMenuItemByID([FromRoute] int ItemID)
        {

            var MenuItem = _menuItemService.GetMenuItemByID(ItemID);

            return Ok(MenuItem);
        }

        [Authorize]
        [HttpGet("AvailableItems/{RestaurantID}")]
        public ActionResult GetAvailableMenuItemsByRestaurantID([FromRoute] int RestaurantID)
        {
            var MenuItems = _menuItemService.GetAvailableMenuItemsByRestaurantID(RestaurantID);

            return Ok(MenuItems);
        }

        [Authorize]
        [HttpGet("NotAvailableItems/{RestaurantID}")]
        public ActionResult GetNotAvailableMenuItemsByRestaurantID([FromRoute] int RestaurantID)
        {
            var MenuItems = _menuItemService.GetNotAvailableMenuItemsByRestaurantID(RestaurantID);

            return Ok(MenuItems);
        }

        [Authorize(Roles = "Admin,RestaurantOwner,RestaurantStaff")]
        [HttpDelete("{ItemID}")]
        public ActionResult DeleteMenuItem([FromRoute] int ItemID)
        {
            var UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
         
            var result = _menuItemService.DeleteMenuItem(ItemID,UserID);

            return Ok(result);
        }



    }
}
