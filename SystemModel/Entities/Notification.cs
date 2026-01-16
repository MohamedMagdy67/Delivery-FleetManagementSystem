using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemModel.Entities
{
    public class Notification
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string? Data { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        #region Navigation Properties
        public virtual User User { get; set; }


        #endregion

    }
}
