using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemModel.Entities
{
    public class DriverLocation
    {
        public int ID { get; set; }
        public int DriverID { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime Timestamp { get; set; }

        #region Navigation Properties
        [ForeignKey(nameof(DriverID))]
        public virtual Driver Driver { get; set; }

        #endregion


    }
}
