using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemDTOS.AuthenticationDTOS
{
    public class AuthDTO
    {
        public DateTime ExpireDate { get; set; }
        public string Token { get; set; }

    }
}
