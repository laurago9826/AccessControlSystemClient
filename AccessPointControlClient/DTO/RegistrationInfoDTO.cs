using AccessPointControlClient.HttpClientHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessPointControlClient.DTO
{
    public class RegistrationInfoDTO : UserInfo
    {
        public string Password { get; set; }

        public RegistrationInfoDTO(string username, List<string> roles, string password)
            : base(null, username, roles)
        {
            this.Password = password;
        }
    }
}
