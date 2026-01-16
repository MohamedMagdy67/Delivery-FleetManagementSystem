using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemModel.Entities
{
    public class RestaurantUsers
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int RestaurantID { get; set; }
        public string RoleInRestaurant { get; set; }
        public DateTime CreatedAt { get; set; }
        #region Navigation Properties
        public virtual User User { get; set; }
        [ForeignKey(nameof(RestaurantID))]
        public virtual Restaurant Restaurant { get; set; }
        #endregion

    }
}
