using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemContext.SystemDbContext;
using SystemDTOS.OrderStatusHistoryDTOS;
using SystemModel.Entities;

namespace ServiceLayer.OrderStatusHistoryServices
{
    public class OrderStatusHistoryService
    {
        private readonly DelivryDB _context;
        public OrderStatusHistoryService(DelivryDB context)
        {
            _context = context; 
        }
        public List<OrderStatusHistoryResponse> GetOrderStatusHistoryByOrderID(int OrderID,int userID)
        {
            var Order = _context.Orders.FirstOrDefault(o => o.ID == OrderID);
            if(Order == null)
            {
                throw new Exception("Order Not Found");
            }
            var User = _context.Users.Include(u => u.RestaurantUsers).FirstOrDefault(u => u.ID == userID);
            if (User == null)
            {
                throw new Exception("User Not Found");
            }
            if(User.Role == UserRole.RestaurantOwner || User.Role == UserRole.RestaurantStaff)
            {
                var ResID = User.RestaurantUsers.FirstOrDefault(r => r.RestaurantID == Order.RestaurantID);
                if(ResID == null)
                {
                    throw new Exception("You Are Not RestaurantUser For This Restaurant");
                }
            }
            var history = _context.OrderStatusHistories.Where(o => o.OrderID == OrderID).ToList();
            List<OrderStatusHistoryResponse> History = new List<OrderStatusHistoryResponse>();
            foreach(var h in history)
            {
                History.Add(new OrderStatusHistoryResponse
                {
                    OrderStatusHistoryID = h.ID,
                    OrderID = h.OrderID,
                    OldStatus = h.OldStatus,
                    NewStatus = h.NewStatus,  
                    TimeStamp = h.Timestamp,
                    ChangedByUserID = h.ChangedByUserID
                });
            }
            return History;
        }
        public List<OrderStatusHistoryResponse> GetAllOrdersHistory(int RestaurantID,int userID)
        {
            var Res = _context.Restaurants.FirstOrDefault(r => r.ID == RestaurantID);
            if (Res == null)
            {
                throw new Exception("Restaurant Not Found");
            }
            var User = _context.Users.Include(u => u.RestaurantUsers).FirstOrDefault(u => u.ID == userID);
            if (User == null)
            {
                throw new Exception("User Not Found");
            }
            if (User.Role == UserRole.RestaurantOwner || User.Role == UserRole.RestaurantStaff)
            {
                var ResID = User.RestaurantUsers.FirstOrDefault(r => r.RestaurantID == RestaurantID);
                if (ResID == null)
                {
                    throw new Exception("You Are Not RestaurantUser For This Restaurant");
                }
            }
           
            var Orders = _context.Orders.Where(o => o.RestaurantID == RestaurantID).Select(o => o.ID).ToList();
            var OrdersHistory = _context.OrderStatusHistories.
                Where(o => Orders.Contains(o.OrderID)).
                OrderBy(o => o.OrderID).
                ToList();
            List<OrderStatusHistoryResponse> OrdersHistoryy = new List<OrderStatusHistoryResponse>();
            foreach (var h in OrdersHistory)
            {
                OrdersHistoryy.Add(new OrderStatusHistoryResponse
                {   
                    OrderStatusHistoryID = h.ID,
                    OrderID = h.OrderID,
                    OldStatus = h.OldStatus,
                    NewStatus = h.NewStatus,
                    TimeStamp = h.Timestamp
                });
                
            }
            return OrdersHistoryy;
            
        }
    }
}
