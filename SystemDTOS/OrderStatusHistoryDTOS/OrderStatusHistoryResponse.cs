using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemDTOS.OrderStatusHistoryDTOS
{
    public class OrderStatusHistoryResponse
    {
        public int OrderStatusHistoryID { get; set; }
        public int OrderID { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public DateTime TimeStamp { get; set; }
        public int ChangedByUserID { get; set; }
    }
}
