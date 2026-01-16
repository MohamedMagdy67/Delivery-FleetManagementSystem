using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServiceLayer.Hubs
{
    public class OrderHub : Hub
    {
        public async Task JoinOrderGroup(int orderID)
        {
            string GroupName = $"Order_{orderID}";
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName);
        }
        public async Task LeaveOrderGroup(int orderID)
        {
            string GroupName = $"Order_{orderID}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName);
        }
        public async Task SendOrderUpdate(int orderID, string message)
        {
            string GroupName = $"Order_{orderID}";
            await Clients.Group(GroupName).SendAsync("ReceiveOrderUpdate", message);
        }
    }
}
