using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemDTOS.AuthenticationDTOS;
using SystemModel.Entities;

namespace ServiceLayer.AuthenticationServices
{
    public interface IAuthService
    {
        public Task<UserDTO> RegisterAsync(RegisterDTO dto); 
        public AuthDTO Login(LoginDTO dto); 


    }
}
