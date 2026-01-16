using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SystemModel.Entities
{
    public class Vehicle
    {
        public int ID { get; set; }
        public string PlateNumber { get; set; }
        public string Type { get; set; }
        public int DriverID { get; set; }
        #region Navigation Properties
        public virtual Driver Driver { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        #endregion
    }
}
