using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemDTOS.RestaurantUsersDTOS
{
    #region Enum RoleInRestaurant
    public enum RoleInRestaurant
    {   
         RestaurantOwner,
         RestaurantStaff
    }
    #endregion

    public class AddRestaurantUserDTO
    {
        public int UserID { get; set; }
        public int RestaurantID { get; set; }
        public RoleInRestaurant RoleInRestaurant { get; set; }
    }
}
