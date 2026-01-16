using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemModel.Entities;
namespace SystemDTOS.AuthenticationDTOS
{
    public class UserDTO
    {
        public string Name { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
