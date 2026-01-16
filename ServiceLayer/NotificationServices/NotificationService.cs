using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SystemContext.SystemDbContext;
using SystemDTOS.NotificationDTOS;
using SystemModel.Entities;

namespace ServiceLayer.NotificationServices
{
    public class NotificationService
    {
        private readonly DelivryDB _context;
        public readonly IHubContext<NotificationHub> _hub;
        public NotificationService(DelivryDB context,IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task CreateNotificationAsync(CreateNotificationDTO dto)
        {
            var notification = new Notification()
            {
                UserID = dto.UserID,
                Body = dto.Body,
                Data = dto.Data,
                Title = dto.Title,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
             _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
           
            await _hub.Clients
                .Group($"User_{dto.UserID}")
                .SendAsync("ReceiveNotification"
                , new NotificationResponseDTO()
            {   
                ID = notification.ID,
                Body = notification.Body,
                Data = notification.Data,
                Title = notification.Title,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead
            });

        }
        public async Task<List<NotificationResponseDTO>> GetUserNotificationsAsync(int userID)
        {
            var Notifications = await _context.Notifications.Where(n => n.UserID == userID).OrderByDescending(n => n.CreatedAt).ToListAsync();
            List<NotificationResponseDTO> notifications = new List<NotificationResponseDTO>();
            foreach (var notification in Notifications)
            {
                notifications.Add(new NotificationResponseDTO()
                {
                    ID = notification.ID,
                    UserID = notification.UserID,
                    Title = notification.Title,
                    Body = notification.Body,
                    Data = notification.Data,
                    CreatedAt = notification.CreatedAt,
                    IsRead  = notification.IsRead

                });
            }
            return notifications;
                
        }
        public async Task MarkNotificationAsReadAsync(int NotificationID , int UserID)
        {
            var n = await _context.Notifications.FirstOrDefaultAsync(n => n.ID == NotificationID && n.UserID == UserID);
            if(n == null)
            {
                throw new Exception("Notification Not Found");
            }
            n.IsRead = true;
           await _context.SaveChangesAsync();
        }
        public async Task<NotificationResponseDTO> GetNotificationByIDAsync(int NotificationID , int UserID)
        {
            var n = await _context.Notifications.FirstOrDefaultAsync(n => n.ID == NotificationID && n.UserID == UserID);
            if(n == null)
            {
                throw new Exception("Notification Not Found");
            }
            return new NotificationResponseDTO()
            {
                ID = NotificationID,
                UserID = UserID,
                Title = n.Title,
                Body = n.Body,
                Data = n.Data,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            };
        }
    }
}
