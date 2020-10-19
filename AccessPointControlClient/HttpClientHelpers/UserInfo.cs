using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessPointControlClient.HttpClientHelpers
{
    public class UserInfo
    {
        public string UserId { get; private set; }
        public string Username { get; private set; }
        public List<string> Roles { get; private set; }

        public UserInfo(string userid, string username, List<string> roles)
        {
            this.UserId = userid;
            this.Username = username;
            this.Roles = roles;
        }
    }

    public class SupportedRoles 
    {
        private static List<string> supportedRoles = new List<string>()
        {
            CAMERA_CLIENT_AUTH,
            USER_CREATION
        };

        public const string CAMERA_CLIENT_AUTH = "CameraClientAuth";
        public const string USER_CREATION = "UserCreation";

        public static bool IsRoleSupported(string roleName)
        {
            return supportedRoles.Contains(roleName);
        }
    }
}
