using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemDTOS.ReviewDTOS
{
    public class ReviewResponseForDriverDTO
    {
        public int ReviewID { get; set; }
        public int FromUserID { get; set; }
        public int ToDriverID { get; set; }
        public decimal Rating { get; set; }
        public int OrderID { get; set; }
        public string? Comment { get; set; }
    }
}
