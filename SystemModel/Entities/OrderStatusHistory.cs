using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemModel.Entities
{
    public class OrderStatusHistory
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public int ChangedByUserID { get; set; }
        public DateTime Timestamp { get; set; }
        #region Navigation Properties
        public virtual Order Order { get; set; }
        [ForeignKey(nameof(ChangedByUserID))]
        public virtual User User { get; set; }

        
        #endregion

    }
}
