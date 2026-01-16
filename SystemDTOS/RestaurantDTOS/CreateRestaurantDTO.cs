using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SystemDTOS.RestaurantDTOS
{
    public class CreateRestaurantDTO
    {

        public string Name { get; set; }
        public string Address { get; set; }
        public string? Phone { get; set; }
        public int? OpeningHours { get; set; }
        public int AreaID { get; set; }

    }
}
