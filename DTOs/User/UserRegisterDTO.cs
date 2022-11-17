using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dot_net.DTOs
{
    public class UserRegisterDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;    
    }
}