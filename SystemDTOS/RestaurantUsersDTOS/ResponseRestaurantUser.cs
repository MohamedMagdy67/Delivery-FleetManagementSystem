using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemDTOS.RestaurantUsersDTOS
{
    public class ResponseRestaurantUser
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int RestaurantID { get; set; }
        public string RoleInRestaurant { get; set; }
        public DateTime CreatedAt { get; set; }
    
    }
}
