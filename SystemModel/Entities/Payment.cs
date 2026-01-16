using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemModel.Entities
{
    public class Payment
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        #region Navigation Properties
        [ForeignKey(nameof(OrderID))]
        public virtual Order Order { get; set; }    

        #endregion


    }
}
