using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemModel.Entities;

namespace SystemDTOS.DriverDTOS
{
    public class UpdateDriverDTO
    {
        public string LicenseNumber { get; set; }
        public decimal? Rating { get; set; }
    }
}
