using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemDTOS.UserDTOS
{
    public class UserResponseDTO
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string userRole { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public bool IsActive { get; set; }

    }
}
