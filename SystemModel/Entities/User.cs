using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemModel.Entities
{
    #region ENUMS
    public enum UserRole
    {
        Admin,
        Customer,
        Driver,
        RestaurantOwner,
        RestaurantStaff
    }
    #endregion
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        [MaxLength(30)]
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }
        #region Navigation Properties
        public virtual ICollection<RestaurantUsers> RestaurantUsers { get; set; } = new HashSet<RestaurantUsers>();
        public virtual ICollection<Order> OrdersAsCustomer { get; set; } = new HashSet<Order>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public virtual Driver Driver { get; set; }
        public virtual ICollection<Log> Logs { get; set; } = new HashSet<Log>();
        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new HashSet<OrderStatusHistory>();

        #endregion

    }

}
