using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemModel.Entities
{
    public class Review
    {
        public int ID { get; set; }
        public decimal Rating { get; set; }
        public string? Comment { get; set; }
        public int OrderID { get; set; }
        public int FromUserID { get; set; }
        public int? ToUserID { get; set; }
        public int? ToRestaurantID { get; set; }
        #region Navigation Properties
        public virtual Order Order { get; set; }
        [ForeignKey(nameof(FromUserID))]
        public virtual User User { get; set; }
        [ForeignKey(nameof(ToUserID))]
        public virtual Driver Driver { get; set; }
        [ForeignKey(nameof(ToRestaurantID))]
        public virtual Restaurant Restaurant { get; set; }

        #endregion

    }
}
