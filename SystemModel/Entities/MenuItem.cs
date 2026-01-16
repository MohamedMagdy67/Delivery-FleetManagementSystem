using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemModel.Entities
{
    public class MenuItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int RestaurantID { get; set; }
        
        #region Navigation Properties
        public virtual Restaurant Restaurant { get; set; }
        #endregion

    }
}
