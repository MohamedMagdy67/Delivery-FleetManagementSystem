using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using SystemModel.Entities;

namespace SystemDTOS.OrderDTOS
{
    public class OrderDTO
    {
        public int OrderID { get; set; }
        public int RestaurantID { get; set; }
        public string ToAddress { get; set; }
        public string FromAddress { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DeliveryFee { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CustomerID { get; set; }
        public string PaymentStatus { get; set; }
        public string Status { get; set; }
        public string PackageType { get; set; }
    }
}
