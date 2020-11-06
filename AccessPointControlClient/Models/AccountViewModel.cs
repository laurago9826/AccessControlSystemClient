using AccessPointControlClient.HttpClientHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccessPointControlClient.Models
{
    public class AccountViewModel
    {
        public UserInfo UserInfo { get; set; }
        public PasswordChangeModel PasswordChange { get; set; }
    }

    public class PasswordChangeModel
    {
        [Required]
        [MinLength(5)]
        public string OldPassword { get; set; }
        [Required]
        [MinLength(5)]
        public string NewPassword { get; set; }
        [Required]
        [MinLength(5)]
        [Compare("NewPassword", ErrorMessage = "Confirm password doesn't match!")]
        public string ConfirmPassword { get; set; }
    }
}
