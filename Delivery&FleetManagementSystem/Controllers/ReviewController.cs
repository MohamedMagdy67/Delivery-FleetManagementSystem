using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ServiceLayer.ReviewServices;
using System.Security.Claims;
using SystemDTOS.RestaurantDTOS;
using SystemDTOS.ReviewDTOS;

namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _reviewService;
        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [Authorize]
        [HttpGet("RestaurantReviews/{RestaurantID}")]
        public ActionResult GetRestaurantReviews([FromRoute] int RestaurantID)
        {
            var reviews = _reviewService.GetRestaurantReviews(RestaurantID);

            return Ok(reviews);
        }

        [Authorize]
        [HttpGet("DriverReviews/{DriverID}")]
        public ActionResult GetDriverReviews([FromRoute] int DriverID)
        {
            var reviews = _reviewService.GetDriverReviews(DriverID);

            return Ok(reviews);
        }

        [Authorize(Roles = "Admin,Customer")]
        [HttpPost("Restaurant")]
        public ActionResult CreateRestaurantReview(CreateReviewForRestaurantDTO dto)
        {
            var FromUserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var NewReview = _reviewService.CreateReviewForRestaurant(dto,FromUserID);

            return Ok(NewReview);
        }

        [Authorize(Roles = "Admin,Customer")]
        [HttpPost("Driver")]
        public ActionResult CreateDriverReview(CreateReviewForDriverDTO dto)
        {
            var FromUserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var NewReview = _reviewService.CreateReviewForDriver(dto,FromUserID);

            return Ok(NewReview);
        }
    }
}
