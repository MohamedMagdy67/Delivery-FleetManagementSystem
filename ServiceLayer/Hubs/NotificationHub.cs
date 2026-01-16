using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
namespace ServiceLayer.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var UserID = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(UserID))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{UserID}");
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var UserID = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(UserID))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{UserID}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
