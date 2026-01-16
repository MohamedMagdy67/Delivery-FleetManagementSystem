using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemDTOS.RestaurantDTOS
{
    public class UpdateRestaurantDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int OpeningHours { get; set; }
        public string Phone { get; set; }
        public int AreaID { get; set; }
    }
}
