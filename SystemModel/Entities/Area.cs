using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemModel.Entities
{
    public class Area
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        #region Navigation Properties
        public virtual ICollection<Restaurant> Restaurants { get; set; } = new HashSet<Restaurant>();

        #endregion

    }
}
