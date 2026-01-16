using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemDTOS.AreaDTOS
{
    public class AreaResponseDTO
    {
        public int AreaID { get; set; }
        public string AreaName { get; set; }
        public string AreaCity { get; set; }
        public decimal AreaLatitude { get; set; }
        public decimal AreaLongitude { get; set; }
    }
}
