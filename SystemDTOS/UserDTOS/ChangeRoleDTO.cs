using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemModel.Entities;

namespace SystemDTOS.UserDTOS
{
    public class ChangeRoleDTO
    {
        public int UserID { get; set; }
        public UserRole NewRole { get; set; }
    }
}
