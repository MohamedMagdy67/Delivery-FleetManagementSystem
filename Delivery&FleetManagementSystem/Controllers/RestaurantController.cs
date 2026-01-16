using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.RestaurantServices;
using System.Text.RegularExpressions;
using SystemDTOS.RestaurantDTOS;

namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        public readonly RestaurantService _restaurantService;
        public RestaurantController(RestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDTO dto)
        {
            var restaurant = _restaurantService.CreateRestaurant(dto);

            return Ok(restaurant);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetRestaurants()
        {
            var Restaurants = _restaurantService.GetRestaurants();

            return Ok(Restaurants);
        }

        [Authorize]
        [HttpGet("{ID}")]
        public ActionResult GetRestaurantByID([FromRoute] int ID)
        {
            var Restaurant = _restaurantService.GetRestaurantByID(ID);

            return Ok(Restaurant);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{ID}/activate")]
        public ActionResult ActivateRestaurant([FromRoute] int ID)
        {
            var Restaurant = _restaurantService.ActivateRestaurant(ID);

            return Ok(Restaurant);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{ID}/deactivate")]
        public ActionResult DeActivateRestaurant([FromRoute] int ID)
        {
            var Restaurant = _restaurantService.DeActivateRestaurant(ID);

            return Ok(Restaurant);
        }

        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpDelete("{ID}")]
        public ActionResult DeleteRestaurant([FromRoute] int ID)
        {
            var result = _restaurantService.DeleteRestaurant(ID);

            return Ok(result);
        }

        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPut("{ID}/update")]
        public ActionResult UpdateRestaurant([FromRoute]int ID,[FromBody]UpdateRestaurantDTO dto)
        {
            var NewRestaurant = _restaurantService.UpdateRestaurant(ID, dto);

            return Ok(NewRestaurant);
        }

    }
}
