using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemDTOS.OrderDTOS
{
    public class CreateOrderDTO
    {
        public int RestaurantID { get; set; }
        public string ToAddress { get; set; }
        public List<CartDTO> Cart { get; set; } = new List<CartDTO>();
    }
}
