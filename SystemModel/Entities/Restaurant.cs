using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemModel.Entities
{
    public class Restaurant
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string? Phone { get; set; }
        public int? OpeningHours { get; set; }
        public bool IsActive { get; set; }
        public int AreaID { get; set; }
        public decimal? Rating { get; set; }
        #region Navigation Properties
        public virtual ICollection<RestaurantUsers> RestaurantUsers { get; set; } = new HashSet<RestaurantUsers>();
        public virtual ICollection<MenuItem> MenuItems { get; set; } = new HashSet<MenuItem>();
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public virtual Area Area { get; set; }
        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        #endregion


    }
}
