using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.RestaurantServices;
using System.Security.Claims;
using SystemDTOS.RestaurantUsersDTOS;

namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantUserController : ControllerBase
    {
        public readonly RestaurantUsersService _restaurantUsersService;
        public RestaurantUserController(RestaurantUsersService restaurantUsersService)
        {
            _restaurantUsersService = restaurantUsersService;
        }
       
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPost]
        public ActionResult AddRestaurantUser([FromBody]AddRestaurantUserDTO dto)
        {
            var UserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var RestaurantUser = _restaurantUsersService.AddRestaurantUser(dto,int.Parse(UserID));

            return Ok(RestaurantUser);

        }

        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpGet("{RestaurantID}")]
        public ActionResult GetRestaurantUsers([FromRoute]int RestaurantID)
        {
            var RestaurantUsers = _restaurantUsersService.GetRestaurantUsers(RestaurantID);
            
            return Ok(RestaurantUsers);
        }

        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpDelete("{RestaurantID}/{UserID}")]
        public ActionResult RemoveRestaurantUser([FromRoute]int RestaurantID,[FromRoute]int UserID)
        {
            var CurrentUserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = _restaurantUsersService.RemoveRestaurantUser(UserID, RestaurantID, int.Parse(CurrentUserID));

            return Ok(result);
        }
        
    }
}
