using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemModel.Entities;

namespace SystemDTOS.DriverDTOS
{
    public class DriverResponseDTO
    {
        public int DriverID { get; set; }
        public string LicenseNumber { get; set; }
        public string Status { get; set; }
        public int UserID { get; set; }
        public decimal? rating { get; set; }
    }
}
