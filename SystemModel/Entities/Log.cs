using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemModel.Entities
{
    public class Log
    {
        public int ID { get; set; }
        public string Action { get; set; }
        public string Entity { get; set; }
        public int EntityID { get; set; }
        public int PerformedByUserID { get; set; }
        public string? Details { get; set; }
        public DateTime CreatedAt { get; set; }

        #region Navigation Properties
        [ForeignKey(nameof(PerformedByUserID))]
        public virtual User User { get; set; }

        #endregion


    }
}
