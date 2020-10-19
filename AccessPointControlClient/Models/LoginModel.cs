using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessPointControlClient.Models
{
    public class LoginModel
    {
        public string ServerIP { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
