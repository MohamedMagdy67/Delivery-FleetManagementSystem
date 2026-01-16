using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemDTOS.VehicleDTOS
{   
    public enum VehicleType
    {
        Bike,
        Motorcycle
    } 
    public class CreateVehicleDTO
    {
        public string PlateNumber { get; set; }
        public VehicleType Type { get; set; }
        public int DriverID { get; set; }
    }
}
