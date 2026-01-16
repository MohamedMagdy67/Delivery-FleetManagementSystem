using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemModel.Entities;

namespace SystemDTOS.DriverDTOS
{
    public class CreateDriverDTO
    {
        public string LicenseNumber { get; set; }
        public int UserID { get; set; }
    }
}
