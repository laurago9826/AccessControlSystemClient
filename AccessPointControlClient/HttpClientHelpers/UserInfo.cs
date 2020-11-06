using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessPointControlClient.HttpClientHelpers
{
    public class UserInfo
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }

        public UserInfo()
        { }
        public UserInfo(string userid, string username, List<string> roles)
        {
            this.Id = userid;
            this.Username = username;
            this.Roles = roles;
        }
    }

    public class SupportedRoles 
    {
        private static List<string> supportedRoles = new List<string>()
        {
            CAMERA_CLIENT_AUTH,
            PERSON_MANAGEMENT,
            USER_ACCOUNT_MANAGEMENT,
            OWN_USER_ACCOUNT_EDIT,
            LOG_ACCESS,
            VIEW_DEBUG
        };

        public const string CAMERA_CLIENT_AUTH = "CameraClientAuth";
        public const string PERSON_MANAGEMENT = "PersonManagement";
        public const string USER_ACCOUNT_MANAGEMENT = "UserAccountManagement";
        public const string OWN_USER_ACCOUNT_EDIT = "OwnUserAccountSettings";
        public const string LOG_ACCESS = "LogAccess";
        public const string VIEW_DEBUG = "ViewDebug";

        public static bool IsRoleSupported(string roleName)
        {
            return supportedRoles.Contains(roleName);
        }

        public static List<string> GetAll()
        {
            return supportedRoles;
        }
    }
}
