using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SystemModel.Entities
{   public enum DriverStatus 
    {
        Active,
        Busy,
        NotActive
    }
    public class Driver
    {
        public int ID { get; set; }
        public string LicenseNumber { get; set; }
        public DriverStatus Status { get; set; }
        public decimal? Rating { get; set; }
        public int UserID { get; set; }

        #region Navigation Properties
        public virtual Vehicle Vehicle { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        [ForeignKey(nameof(UserID))]
        public virtual User User { get; set; }
        public virtual DriverLocation DriverLocation { get; set; }
        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();


        #endregion
    }
}
