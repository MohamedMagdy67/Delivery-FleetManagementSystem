using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemModel.Entities;
namespace SystemContext.SystemDbContext
{
    public class DelivryDB : DbContext
    {
        public DelivryDB(DbContextOptions<DelivryDB> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region USERS INFORMATION
            modelBuilder.Entity<User>()
                .Property(u => u.Name).HasMaxLength(150).IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.Email).HasMaxLength(200).IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash).IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.Role).HasConversion<string>().IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt).IsRequired();

            modelBuilder.Entity<User>().HasMany(u => u.OrdersAsCustomer).WithOne(O => O.Customer).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>().HasMany(u => u.RestaurantUsers).WithOne(R => R.User).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>().HasMany(u => u.Notifications).WithOne(N => N.User).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>().HasOne(u => u.Driver).WithOne(O => O.User).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>().HasMany(u => u.OrdersAsCustomer).WithOne(O => O.Customer).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region AREA INFORMATION
            modelBuilder.Entity<Area>().Property(a => a.Name).IsRequired();
            modelBuilder.Entity<Area>().Property(a => a.City).IsRequired();
            modelBuilder.Entity<Area>().Property(a => a.Latitude).IsRequired();
            modelBuilder.Entity<Area>().Property(a => a.Longitude).IsRequired();

            modelBuilder.Entity<Area>().HasMany(a => a.Restaurants).WithOne(r => r.Area).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region DRIVER INFORMATION
            modelBuilder.Entity<Driver>().Property(d => d.LicenseNumber).IsRequired();
            modelBuilder.Entity<Driver>().Property(d => d.Status).HasConversion<string>().IsRequired();
            modelBuilder.Entity<Driver>().Property(d => d.UserID).IsRequired();

            modelBuilder.Entity<Driver>().HasMany(d => d.Orders).WithOne(o => o.Driver).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Driver>().HasOne(d => d.User).WithOne(u => u.Driver).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Driver>().HasOne(d => d.DriverLocation).WithOne(d => d.Driver).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Driver>().HasOne(d => d.Vehicle).WithOne(v => v.Driver).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region DRIVERLOCATION INFORMATION
            modelBuilder.Entity<DriverLocation>().Property(d => d.DriverID).IsRequired();
            modelBuilder.Entity<DriverLocation>().Property(d => d.Latitude).IsRequired();
            modelBuilder.Entity<DriverLocation>().Property(d => d.Longitude).IsRequired();
            modelBuilder.Entity<DriverLocation>().Property(d => d.Timestamp).IsRequired();

            modelBuilder.Entity<DriverLocation>().HasOne(d => d.Driver).WithOne(d => d.DriverLocation).OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Log INFORMATION
            modelBuilder.Entity<Log>().Property(l => l.Action).IsRequired();
            modelBuilder.Entity<Log>().Property(l => l.Entity).IsRequired();
            modelBuilder.Entity<Log>().Property(l => l.EntityID).IsRequired();
            modelBuilder.Entity<Log>().Property(l => l.PerformedByUserID).IsRequired();

            modelBuilder.Entity<Log>().HasOne(l => l.User).WithMany(u => u.Logs).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region MenuItem INFORMATION
            modelBuilder.Entity<MenuItem>().Property(m => m.Name).IsRequired();
            modelBuilder.Entity<MenuItem>().Property(m => m.Price).IsRequired();
            modelBuilder.Entity<MenuItem>().Property(m => m.IsAvailable).IsRequired();
            modelBuilder.Entity<MenuItem>().Property(m => m.RestaurantID).IsRequired();

            modelBuilder.Entity<MenuItem>().HasOne(m => m.Restaurant).WithMany(r => r.MenuItems).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Notification INFORMATION
            modelBuilder.Entity<Notification>().Property(n => n.UserID).IsRequired();
            modelBuilder.Entity<Notification>().Property(n => n.Body).IsRequired();
            modelBuilder.Entity<Notification>().Property(n => n.Title).IsRequired();
            modelBuilder.Entity<Notification>().Property(n => n.IsRead).IsRequired();
            modelBuilder.Entity<Notification>().Property(n => n.CreatedAt).IsRequired();

            modelBuilder.Entity<Notification>().HasOne(n => n.User).WithMany(u => u.Notifications).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Order INFORMATION
            modelBuilder.Entity<Order>().Property(o => o.CustomerID).IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.RestaurantID).IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.DriverID).IsRequired(false);
            modelBuilder.Entity<Order>().Property(o => o.VehicleID).IsRequired(false);
            modelBuilder.Entity<Order>().Property(o => o.FromAddress).IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.ToAddress).IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.PackageType).IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.TotalPrice).IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.DeliveryFee).IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.Status).HasConversion<string>().IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.PaymentStatus).HasConversion<string>().IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.CreatedAt).IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.EstimatedPickupAt).IsRequired(false);
            modelBuilder.Entity<Order>().Property(o => o.DeliveredAt).IsRequired(false);

            modelBuilder.Entity<Order>().HasOne(o => o.Customer).WithMany(c => c.OrdersAsCustomer).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>().HasOne(o => o.Driver).WithMany(d => d.Orders).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>().HasOne(o => o.Vehicle).WithMany(v => v.Orders).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>().HasOne(o => o.Restaurant).WithMany(r => r.Orders).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>().HasMany(o => o.OrderStatusHistories).WithOne(s => s.Order).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>().HasOne(o => o.Payment).WithOne(s => s.Order).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>().HasMany(o => o.Reviews).WithOne(s => s.Order).OnDelete(DeleteBehavior.Restrict);
           
            #endregion

            #region OrderStatusHistory INFORMATION
            modelBuilder.Entity<OrderStatusHistory>().Property(o => o.OrderID).IsRequired();
            modelBuilder.Entity<OrderStatusHistory>().Property(o => o.OldStatus).IsRequired();
            modelBuilder.Entity<OrderStatusHistory>().Property(o => o.NewStatus).IsRequired();
            modelBuilder.Entity<OrderStatusHistory>().Property(o => o.Timestamp).IsRequired();
            modelBuilder.Entity<OrderStatusHistory>().Property(o => o.ChangedByUserID).IsRequired();

            modelBuilder.Entity<OrderStatusHistory>().HasOne(o => o.Order).WithMany(o => o.OrderStatusHistories).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderStatusHistory>().HasOne(o => o.User).WithMany(u => u.OrderStatusHistories).OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Payment INFORMATION
            modelBuilder.Entity<Payment>().Property(p => p.OrderID).IsRequired();
            modelBuilder.Entity<Payment>().Property(p => p.Amount).IsRequired();
            modelBuilder.Entity<Payment>().Property(p => p.Method).IsRequired();
            modelBuilder.Entity<Payment>().Property(p => p.Status).IsRequired();

            modelBuilder.Entity<Payment>().HasOne(p => p.Order).WithOne(o => o.Payment).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Restaurant INFORMATION
            modelBuilder.Entity<Restaurant>().Property(r => r.Name).IsRequired();
            modelBuilder.Entity<Restaurant>().Property(r => r.Address).IsRequired();
            modelBuilder.Entity<Restaurant>().Property(r => r.Phone).IsRequired(false);
            modelBuilder.Entity<Restaurant>().Property(r => r.OpeningHours).IsRequired(false);
            modelBuilder.Entity<Restaurant>().Property(r => r.IsActive).IsRequired();
            modelBuilder.Entity<Restaurant>().Property(r => r.AreaID).IsRequired();

            modelBuilder.Entity<Restaurant>().HasMany(r => r.MenuItems).WithOne(m => m.Restaurant).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Restaurant>().HasMany(r => r.RestaurantUsers).WithOne(m => m.Restaurant).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Restaurant>().HasMany(r => r.Orders).WithOne(m => m.Restaurant).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Restaurant>().HasOne(r => r.Area).WithMany(m => m.Restaurants).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Restaurant>().HasMany(r => r.Reviews).WithOne(r => r.Restaurant).HasForeignKey(r => r.ToRestaurantID).OnDelete(DeleteBehavior.Restrict);
            
            #endregion

            #region RestaurantUsers INFORMATION
            modelBuilder.Entity<RestaurantUsers>().Property(r => r.RestaurantID).IsRequired();
            modelBuilder.Entity<RestaurantUsers>().Property(r => r.UserID).IsRequired();
            modelBuilder.Entity<RestaurantUsers>().Property(r => r.CreatedAt).IsRequired();
            modelBuilder.Entity<RestaurantUsers>().Property(r => r.RoleInRestaurant).IsRequired();

            modelBuilder.Entity<RestaurantUsers>().HasOne(r => r.User).WithMany(u => u.RestaurantUsers).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RestaurantUsers>().HasOne(r => r.Restaurant).WithMany(u => u.RestaurantUsers).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Review INFORMATION
            modelBuilder.Entity<Review>().Property(r => r.Rating).IsRequired();
            modelBuilder.Entity<Review>().Property(r => r.FromUserID).IsRequired();
            modelBuilder.Entity<Review>().Property(r => r.ToUserID).IsRequired();
            modelBuilder.Entity<Review>().Property(r => r.OrderID).IsRequired();
            modelBuilder.Entity<Review>().Property(r => r.ToRestaurantID).IsRequired(false);
            modelBuilder.Entity<Review>().Property(r => r.ToUserID).IsRequired(false);

            modelBuilder.Entity<Review>().HasOne(r => r.Order).WithMany(o => o.Reviews).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Review>().HasOne(r => r.User).WithMany(o => o.Reviews).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Review>().HasOne(r => r.Driver).WithMany(o => o.Reviews).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Vehicle INFORMATION
            modelBuilder.Entity<Vehicle>().Property(v => v.PlateNumber).IsRequired();
            modelBuilder.Entity<Vehicle>().Property(v => v.Type).IsRequired();
            modelBuilder.Entity<Vehicle>().Property(v => v.DriverID).IsRequired();

            modelBuilder.Entity<Vehicle>().HasOne(v => v.Driver).WithOne(d => d.Vehicle).OnDelete(DeleteBehavior.Restrict); 
            #endregion


        }

        #region DbSet
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Restaurant> Restaurants { get; set; }
        public virtual DbSet<RestaurantUsers> RestaurantUsers { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<DriverLocation> DriverLocations { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<MenuItem> MenuItems { get; set; }
        public virtual DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

        #endregion

    }
}
