using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.NotificationServices;
using System.Security.Claims;

namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetNotificationsAsync()
        {
            var UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var Notifications = await _notificationService.GetUserNotificationsAsync(UserID);

            return Ok(Notifications);
        }
        [Authorize]
        [HttpGet("NotificationByID/{NotificationID}")]
        public async Task<ActionResult> GetNotificationByIDAsync([FromRoute] int NotificationID)
        {
            var UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var Notifications = await _notificationService.GetNotificationByIDAsync(NotificationID, UserID);

            return Ok(Notifications);
        }
        [Authorize]
        [HttpPut("{NotificationID}")]
        public async Task<ActionResult> Read([FromRoute] int NotificationID)
        {
            var UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _notificationService.MarkNotificationAsReadAsync(NotificationID,UserID);

            return Ok(new { message = "Notification marked as read" });
        }

    }
}
