using Microsoft.AspNetCore.SignalR;
using ServiceLayer.LocationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemDTOS.LocationDTOS;
using SystemContext.SystemDbContext;
using Microsoft.EntityFrameworkCore;
using SystemModel.Entities;

namespace ServiceLayer.Hubs
{
    public class DriverTrackingHub : Hub
    {
        private readonly LocationService _locationService;
        private readonly DelivryDB _context;

        public DriverTrackingHub(LocationService locationService, DelivryDB context)
        {
            _locationService = locationService;
            _context = context;
        }

        public async Task UpdateDriverLocation(LocationDTO dto)
        {
           var d = await _locationService.UpdateLocationAsync(dto);

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Restaurant)
                .ThenInclude(r => r.RestaurantUsers)
                .FirstOrDefaultAsync(o => o.DriverID == dto.DriverID &&
                                          o.Status != OrderStatus.Delivered &&
                                          o.Status != OrderStatus.Cancelled);

            if (order != null && d >=50)
            {

                string customerGroup = $"User_{order.CustomerID}";

                var RestaurantUsers = order.Restaurant.RestaurantUsers
                    .Select(u => $"User_{u.UserID}")
                    .ToList();

                await Clients.Group(customerGroup)
                    .SendAsync("ReceiveDriverLocation", dto);
                await Clients.Groups(RestaurantUsers.ToArray())
                    .SendAsync("ReceiveDriverLocation", dto);
            }
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
