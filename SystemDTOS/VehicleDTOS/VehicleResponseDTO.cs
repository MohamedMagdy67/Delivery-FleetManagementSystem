using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemDTOS.VehicleDTOS
{
    public class VehicleResponseDTO
    {
        public int VehicleID { get; set; }
        public string PlateNumber { get; set; }
        public string Type { get; set; }
        public int DriverID { get; set; }
    }
}
