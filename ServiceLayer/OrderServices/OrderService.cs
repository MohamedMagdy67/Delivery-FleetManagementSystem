using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemModel.Entities;
using SystemDTOS.OrderDTOS;
using SystemContext.SystemDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using ServiceLayer.Hubs;
using ServiceLayer.NotificationServices;
using SystemDTOS.NotificationDTOS;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ServiceLayer.LocationServices;
using SystemDTOS.LocationDTOS;
using System.Data;
namespace ServiceLayer.OrderServices
{
    public class OrderService
    {
        public readonly DelivryDB _context;
        public readonly IHubContext<OrderHub> _orderHub;
        public readonly NotificationService _notificationService;
        public readonly LocationService _locationService;
        public OrderService(DelivryDB context, IHubContext<OrderHub> orderHub, NotificationService notificationService, LocationService locationService)
        {
            _context = context;
           _orderHub = orderHub;
            _notificationService = notificationService;
            _locationService = locationService;
        }

        //Role => Customer
        public async Task<OrderDTO> CreateOrder(CreateOrderDTO dto, int userID)
        {
            decimal delivryfee = 30;
            var User = _context.Users.Find(userID);
            if (User == null) { throw new Exception("This User IS Not Exist"); }
            if (User.Role != UserRole.Customer) { throw new Exception("Only Customer Can Make Order"); }
            Restaurant? Restaurant = _context.Restaurants.Find(dto.RestaurantID);
            if (Restaurant == null)
            {
                throw new Exception("Invaild Restaurant");
            }
            if (Restaurant.IsActive == false) { throw new Exception("This Restaurant Is Not Active Now"); }
            List<int> ItemIDs = new List<int>();
            List<int> ItemQuantity = new List<int>();
            foreach(var i in dto.Cart)
            {
                ItemIDs.Add(i.ItemID);
                ItemQuantity.Add(i.Quantity);
            }
            var items = _context.MenuItems.Where(i => ItemIDs.Contains(i.ID)).ToList();
            decimal price = 0;
            int C = 0;
            string Packagetype = "";
            foreach(var i in items)
            {
                Packagetype += $"{ItemQuantity[C]} {i.Name},";
                price += (decimal)i.Price * ItemQuantity[C];
                C++;
            }
            Order order = new Order()
            {
                PackageType = Packagetype,
                RestaurantID = dto.RestaurantID,
                ToAddress = dto.ToAddress,
                FromAddress = Restaurant.Address,
                TotalPrice = delivryfee + price,
                DeliveryFee = delivryfee,
                CreatedAt = DateTime.UtcNow,
                CustomerID = User.ID,
                PaymentStatus = PaymentStatus.Pending,
                Status = OrderStatus.Pending
            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            OrderDTO dtoo = new OrderDTO()
            {   OrderID = order.ID,
                PackageType = order.PackageType,
                RestaurantID = order.RestaurantID,
                ToAddress = order.ToAddress,
                FromAddress = Restaurant.Address,
                TotalPrice = order.TotalPrice,
                DeliveryFee = delivryfee,
                CreatedAt = DateTime.UtcNow,
                CustomerID = User.ID,
                PaymentStatus = PaymentStatus.Pending.ToString(),
                Status = OrderStatus.Pending.ToString()
            };
            var Log = new Log()
            {
                Action = "Create Order",
                Entity = "Order",
                EntityID = order.ID,
                Details = "Cutomer Created Order",
                PerformedByUserID = User.ID,
                CreatedAt = order.CreatedAt

            };
            var n = new CreateNotificationDTO()
            {   UserID = User.ID,
                Title = "Order Status",
                Body = "Order Created",
                Data = "Order Sent To Restaurant and Will Aceept Soon",
            };
            var RestaurantUsers = _context.RestaurantUsers.Where(r => r.RestaurantID == order.RestaurantID).ToList();
            foreach (var r in RestaurantUsers)
            {
                var nR = new CreateNotificationDTO()
                {
                    UserID = r.UserID,
                    Title = "Order Status",
                    Body = "Order Created",
                    Data = "Customer Has Created Order From You Accept This Order",
                };
                await _notificationService.CreateNotificationAsync(nR);

            }
            var OrderStatusHistory = new OrderStatusHistory
            {
                OrderID = order.ID,
                OldStatus = "No Status",
                NewStatus = "Created",
                ChangedByUserID = User.ID,
                Timestamp = DateTime.UtcNow,
            };
            var Payment = new Payment
            {
                OrderID = order.ID,
                Method = "cash",
                Status = PaymentStatus.Pending.ToString(),
                Amount = order.TotalPrice

            };
            _context.Payments.Add(Payment);
            await _notificationService.CreateNotificationAsync(n);
            _context.Logs.Add(Log);
            _context.OrderStatusHistories.Add(OrderStatusHistory);
            await _context.SaveChangesAsync();
            await _orderHub.Clients.Group($"Order_{order.ID}")
                .SendAsync("ReceiveOrderUpdate", new { Status = order.Status.ToString() });
            return dtoo;

        }
        //Role => RestaurantOwner,RestaurantStaff
        public async Task AcceptOrderAsync(int OrderID, int userID)
        {
            var User = _context.Users.Include(u => u.RestaurantUsers).FirstOrDefault(u => u.ID == userID);
            if (User == null)
            {
                throw new Exception("This User IS Not Exist");
            }
            if (User.Role == UserRole.RestaurantStaff || User.Role == UserRole.RestaurantOwner)
            {
                Order? order = _context.Orders.Find(OrderID);
                if (order == null) { throw new Exception("This Order Is Not Exist"); }
                int resID = order.RestaurantID;
                bool statuss = false;
                foreach(var r in User.RestaurantUsers)
                { 
                    var resIDD = r.RestaurantID; 
                    if(resIDD == resID)
                    {
                        statuss = true;
                        break;
                    }
                }
                if (statuss == false) 
                {
                    throw new Exception("You Are Not Able To Accept This Order"); 
                }
               order.AcceptOrder();

                var Log = new Log()
                {
                    Action = "Accept Order",
                    Entity = "Order",
                    EntityID = order.ID,
                    Details = "Restaurant Accepted Order",
                    PerformedByUserID = User.ID,
                    CreatedAt = DateTime.UtcNow

                };
                var n = new CreateNotificationDTO()
                {
                    UserID = order.CustomerID,
                    Title = "Order Status",
                    Body = "Order Accepted",
                    Data = "Restaurant Making Your Order Now",
                };
                var RestaurantUsers = _context.RestaurantUsers.Where(r => r.RestaurantID == order.RestaurantID).ToList();
                foreach (var r in RestaurantUsers)
                {
                    var nR = new CreateNotificationDTO()
                    {
                        UserID = r.UserID,
                        Title = "Order Status",
                        Body = "Order Accepted",
                        Data = "When It Be Ready Tell Us",
                    };
                    await _notificationService.CreateNotificationAsync(nR);

                }
                var OrderStatusHistory = new OrderStatusHistory
                {
                    OrderID = order.ID,
                    OldStatus = "Created",
                    NewStatus = "Accepted",
                    ChangedByUserID = User.ID,
                    Timestamp = DateTime.UtcNow,
                };
                await _notificationService.CreateNotificationAsync(n);
                _context.Logs.Add(Log);
                _context.OrderStatusHistories.Add(OrderStatusHistory);
                await _context.SaveChangesAsync();
                await _orderHub.Clients.Group($"Order_{order.ID}")
             .SendAsync("ReceiveOrderUpdate", new { Status = order.Status.ToString() });

            }
            else
            {
                throw new Exception("Only RestaurantOwner And RestaurantStaff Can Accept The Order");
            }
        }
        //Role => Admin
        public async Task AssignDriverAsync(int OrderID,int userID)
        {
            var user = _context.Users.Include(u => u.RestaurantUsers).FirstOrDefault(u => u.ID == userID);
            if(user == null)
            {
                throw new Exception("User Not Found");
            }
            if(user.Role != UserRole.RestaurantStaff && user.Role != UserRole.RestaurantOwner)
            {
                throw new Exception("Invalid User");
            }
            Order? order = _context.Orders.Find(OrderID);
            if (order == null)
            {
                throw new Exception("This Order Is Not Exist");
            }
            var ResID = user.RestaurantUsers.FirstOrDefault(u => u.RestaurantID == order.RestaurantID);
            if (ResID == null)
            {
                throw new Exception("This User Is Not The Owner Or RestaurantStaff On This Restaurant");
            }
            var Restaurant = _context.Restaurants.Include(r => r.Area).FirstOrDefault(r => r.ID == order.RestaurantID);
            int driverID = await GetNearestDriverAsync(Restaurant.Area.Longitude, Restaurant.Area.Latitude);
            if(driverID == 0)
            {
                throw new Exception("No Available Drivers");
            }
            var Driver = _context.Drivers.Include(d => d.Vehicle).FirstOrDefault(d => d.ID == driverID);
            if (Driver == null)
            {
                throw new Exception("This Driver Is Not Exist");
            }
            if (Driver.Status == DriverStatus.Busy || Driver.Status == DriverStatus.NotActive)
            {
                throw new Exception("This Driver Is Not Available Now");
            }
          
            order.AssignDriver(driverID);
            order.DriverID = driverID;
            order.VehicleID = Driver.Vehicle.ID;
            Driver.Status = DriverStatus.Busy;
            var nC = new CreateNotificationDTO()
            {
                UserID = order.CustomerID,
                Title = "Order Status",
                Body = "Driver Assigned To Your Order",
                Data = "Order Finished And Driver Assigned",
            };
            var nD = new CreateNotificationDTO()
            {
                UserID = Driver.UserID,
                Title = "Order Status",
                Body = "You Are Assigned To Order And Order Is Ready For PickUp",
                Data = "Go To PickUp",
            };
            var RestaurantUsers = _context.RestaurantUsers.Where(r => r.RestaurantID == order.RestaurantID).ToList();
            foreach(var r in RestaurantUsers)
            {
                var nR = new CreateNotificationDTO()
                {
                    UserID = r.UserID,
                    Title = "Order Status",
                    Body = "Driver Assigned To Your Order",
                    Data = "Driver Coming To PickUp The Order",
                };
                await _notificationService.CreateNotificationAsync(nR);

            }
            var Log = new Log()
            {
                Action = "Assign Driver",
                Entity = "Order",
                EntityID = order.ID,
                Details = "RestaurantUser Assigned Driver For Order",
                PerformedByUserID = userID,
                CreatedAt = DateTime.UtcNow

            };
            var OrderStatusHistory = new OrderStatusHistory
            {
                OrderID = order.ID,
                OldStatus = "Accepted",
                NewStatus = "Driver Assigned and Order Is Ready For Pickup",
                ChangedByUserID = userID,
                Timestamp = DateTime.UtcNow,
            };
            await _notificationService.CreateNotificationAsync(nC);
            await _notificationService.CreateNotificationAsync(nD);
            _context.OrderStatusHistories.Add(OrderStatusHistory);
            _context.Logs.Add(Log);
            await _context.SaveChangesAsync();
            
            await _orderHub.Clients.Group($"Order_{order.ID}")
             .SendAsync("ReceiveOrderUpdate", new { Status = order.Status.ToString() });



        }
        //Role =>  Driver
        public async Task PickupOrderAsync(int OrderID, int userID)
        {
            var User = _context.Users.Find(userID);
            if (User == null)
            {
                throw new Exception("This User Is Not Exist");
            }
            if (User.Role != UserRole.Driver)
            {
                throw new Exception("Only Driver Can Pickup The Order");
            }
            Driver driver = _context.Drivers.FirstOrDefault(d => d.UserID == User.ID);
            if (driver == null) { throw new Exception("This User Is Not An Driver"); }
            Order? order = _context.Orders.Find(OrderID);
            if (order == null)
            {
                throw new Exception("This Order Is Not Exist");
            }
            if (order.DriverID != driver.ID)
            {
                throw new Exception("You Are Not Assigned Driver For This Order");

            }
            order.PickupOrder();
            var Log = new Log()
            {
                Action = "Pickup Order",
                Entity = "Order",
                EntityID = order.ID,
                Details = "Driver Pickedup Order",
                PerformedByUserID = User.ID,
                CreatedAt = DateTime.UtcNow

            };
            var nC = new CreateNotificationDTO()
            {
                UserID = order.CustomerID,
                Title = "Order Status",
                Body = "Order PickedUp",
                Data = "Driver Has PickedUp Your Order And Going To You",
            };
            var nD = new CreateNotificationDTO()
            {
                UserID = driver.UserID,
                Title = "Order Status",
                Body = "You PickedUp The Order",
                Data = "Deliver It",
            };
            var RestaurantUsers = _context.RestaurantUsers.Where(r => r.RestaurantID == order.RestaurantID).ToList();
            foreach (var r in RestaurantUsers)
            {
                var nR = new CreateNotificationDTO()
                {
                    UserID = r.UserID,
                    Title = "Order Status",
                    Body = "Order PickedUp",
                    Data = "Driver Going To Deliver It",
                };
                await _notificationService.CreateNotificationAsync(nR);

            }
            var OrderStatusHistory = new OrderStatusHistory
            {
                OrderID = order.ID,
                OldStatus = "Ready For PickUp",
                NewStatus = "PickedUp",
                ChangedByUserID = User.ID,
                Timestamp = DateTime.UtcNow,
            };
            await _notificationService.CreateNotificationAsync(nD);
            await _notificationService.CreateNotificationAsync(nC);
            _context.Logs.Add(Log);
            _context.OrderStatusHistories.Add(OrderStatusHistory);
            await _context.SaveChangesAsync();
            await _orderHub.Clients.Group($"Order_{order.ID}")
             .SendAsync("ReceiveOrderUpdate", new { Status = order.Status.ToString() });

        }
        //Role =>  Driver
        public async Task DeliverOrderAsync(int OrderID, int userID)
        {
            var User = _context.Users.Find(userID);
            if (User == null)
            {
                throw new Exception("This User Is Not Exist");
            }
            if (User.Role != UserRole.Driver)
            {
                throw new Exception("Only Driver Can Deliver The Order");
            }
            Driver? driver = _context.Drivers.FirstOrDefault(d => d.UserID == User.ID);
            if (driver == null) { throw new Exception("This User Is Not An Driver"); }
            Order? order = _context.Orders.Find(OrderID);
            if (order == null)
            {
                throw new Exception("This Order Is Not Exist");
            }
            if (order.DriverID != driver.ID)
            {
                throw new Exception("You Are Not Assigned Driver For This Order");

            }
            order.DeliverOrder();
            order.PaymentStatus = PaymentStatus.Paid;
            order.DeliveredAt = DateTime.UtcNow;
            driver.Status = DriverStatus.Active;
            var Log = new Log()
            {
                Action = "Deliver Order",
                Entity = "Order",
                EntityID = order.ID,
                Details = "Driver Delivred Order",
                PerformedByUserID = User.ID,
                CreatedAt = DateTime.UtcNow

            };
            var n = new CreateNotificationDTO()
            {
                UserID = order.CustomerID,
                Title = "Order Status",
                Body = "Order Delivred",
                Data = "Order Delivred To You Enjoy",
            };
            var RestaurantUsers = _context.RestaurantUsers.Where(r => r.RestaurantID == order.RestaurantID).ToList();
            foreach (var r in RestaurantUsers)
            {
                var nR = new CreateNotificationDTO()
                {
                    UserID = r.UserID,
                    Title = "Order Status",
                    Body = "Order Delivred Succesfully",
                    Data = "Driver Has Delivred The Order",
                };
                await _notificationService.CreateNotificationAsync(nR);

            }
            var nD = new CreateNotificationDTO()
            {
                UserID = driver.UserID,
                Title = "Order Status",
                Body = "You Delivred The Order",
                Data = "Order Done",
            };
            var OrderStatusHistory = new OrderStatusHistory
            {
                OrderID = order.ID,
                OldStatus = "PickedUp",
                NewStatus = "Delivred",
                ChangedByUserID = User.ID,
                Timestamp = DateTime.UtcNow,
            };
            var payment = _context.Payments.FirstOrDefault(p => p.OrderID == order.ID);
            if(payment == null)
            {
                throw new Exception("Payment Not Found");
            }
            payment.Status = PaymentStatus.Paid.ToString();
            await _notificationService.CreateNotificationAsync(nD);
            await _notificationService.CreateNotificationAsync(n);
            _context.Logs.Add(Log);
            _context.OrderStatusHistories.Add(OrderStatusHistory);
            await _context.SaveChangesAsync();
            await _orderHub.Clients.Group($"Order_{order.ID}")
             .SendAsync("ReceiveOrderUpdate", new { Status = order.Status.ToString() });


        }
        //Role => Customer,RestaurantOwner,RestaurantStaff
        public async Task CancelOrderAsync(int orderId, string reason, int userID)
        {
            var user = _context.Users.Find(userID);
            if (user == null)
                throw new Exception("User does not exist");

            var order = _context.Orders.Find(orderId);
            if (order == null)
                throw new Exception("Order does not exist");

            // Customer cancels ONLY his own order
            if (user.Role == UserRole.Customer)
            {
                if (order.CustomerID != user.ID)
                    throw new Exception("You cannot cancel an order that is not yours");
            }
            // Restaurant staff / owner cancels ONLY restaurant orders
            else if (user.Role == UserRole.RestaurantOwner || user.Role == UserRole.RestaurantStaff)
            {
                var Restaurantuser = _context.RestaurantUsers.FirstOrDefault(r => r.RestaurantID == order.RestaurantID && r.UserID == user.ID );
                if(Restaurantuser == null)
                {
                    throw new Exception("You Are Not A Member For This Restaurant");
                }
            }
            else
            {
                throw new Exception("You are not allowed to cancel this order");
            }
            var OrderStatusHistory = new OrderStatusHistory
            {
                OrderID = order.ID,
                OldStatus = order.Status.ToString(),
                NewStatus = "Canceled",
                ChangedByUserID = user.ID,
                Timestamp = DateTime.UtcNow,
            };
            order.CancelOrder(); // Domain Rule
            order.PaymentStatus = PaymentStatus.Failed;
            var Log = new Log()
            {
                Action = "Cancel Order",
                Entity = "Order",
                EntityID = order.ID,
                Details = $"{user.Role} Cancelled Order",
                PerformedByUserID = user.ID,
                CreatedAt = DateTime.UtcNow

            };
            var n = new CreateNotificationDTO()
            {
                UserID = user.ID,
                Title = "Order Status",
                Body = "Order Canceled",
                Data = "Your Order Has been Canceled Succesfully",
            };
            var RestaurantUsers = _context.RestaurantUsers.Where(r => r.RestaurantID == order.RestaurantID).ToList();
            foreach (var r in RestaurantUsers)
            {
                var nR = new CreateNotificationDTO()
                {
                    UserID = r.UserID,
                    Title = "Order Status",
                    Body = "Customer Has Canceled The Order",
                    Data = "Order Canceled",
                };
                await _notificationService.CreateNotificationAsync(nR);

            }
            var payment = _context.Payments.FirstOrDefault(p => p.OrderID == order.ID);
            if(payment == null)
            {
                throw new Exception("Payment Not Found");
            }
            payment.Status = PaymentStatus.Failed.ToString();
            await _notificationService.CreateNotificationAsync(n);
            _context.Logs.Add(Log);
            _context.OrderStatusHistories.Add(OrderStatusHistory);
            await _context.SaveChangesAsync();
            await _orderHub.Clients.Group($"Order_{order.ID}")
             .SendAsync("ReceiveOrderUpdate", new { Status = order.Status.ToString() });

        }
        public async Task<int> GetNearestDriverAsync(decimal RestaurantLongtude,decimal RestaurantLatitude)
        {
            var DriversLocation =await _locationService.GetAvailableDriversLocationAsync();
            double MinDistance = 0;
            int DriverID = 0;
            foreach(var d in DriversLocation)
            {
                if(MinDistance == 0)
                {
                    MinDistance = _locationService.GetDistanceInMeters(RestaurantLatitude, RestaurantLongtude, d.Latitude, d.Longitude);
                    DriverID = d.DriverID;
                }
                else
                {
                    var distance = _locationService.GetDistanceInMeters(RestaurantLatitude, RestaurantLongtude, d.Latitude, d.Longitude);
                    if(distance < MinDistance)
                    {
                        MinDistance = distance;
                        DriverID = d.DriverID;
                    }
                }
            }
            return DriverID;

        }
        

    }
}
