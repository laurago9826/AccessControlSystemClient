using AccessPointControlClient.HttpClientHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccessPointControlClient.Models
{
    public class UsersModel
    {
        public UserDetailsModel SelectedUser { get; set; }
        public List<UserDetailsModel> Users { get; set; }

        public UserRegistrationModel UserRegistrationModel { get; set; }
    }


    public class UserDetailsModel : UserInfo 
    {
        public List<RoleValues> RoleValues { get; set; }

        public UserDetailsModel()
        {
            RoleValues = new List<RoleValues>();
            List<string> allRoles = SupportedRoles.GetAll();
            allRoles.ForEach(r => RoleValues.Add(new RoleValues(false, r)));
        }
        public UserDetailsModel(UserInfo user) : base(user.Id, user.Username, user.Roles)
        {
            RoleValues = new List<RoleValues>();
            List<string> allRoles = SupportedRoles.GetAll();
            allRoles.ForEach(r => RoleValues.Add(new RoleValues(false, r)));

            Roles.ForEach(r => 
            {
                RoleValues role = RoleValues.FirstOrDefault(rr => rr.RoleName == r);
                if (role != null)
                {
                    role.IsSelected = true;
                }
            });
        }
    }

    public class UserRegistrationModel : UserDetailsModel
    {
        [Required]
        [MinLength(5)]
        public string Password { get; set; }
        [Required]
        [MinLength(5)]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match!")]
        public string ConfirmPassword { get; set; }

        public UserRegistrationModel()
        { }
    }

    public class RoleValues
    {
        public bool IsSelected { get; set; }
        public string RoleName { get; set; }

        public RoleValues()
        { }
        public RoleValues(bool isSelected, string roleName)
        {
            this.IsSelected = isSelected;
            this.RoleName = roleName;
        }
    }
}
