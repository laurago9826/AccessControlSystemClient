using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AccessPointControlClient.Models;
using System.Net.Http;
using AccessPointControlClient.HttpClientHelpers;
using Microsoft.AspNetCore.Authorization;

namespace AccessPointControlClient.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private const string USERINFO_ENDPOINT = "accounts/user-info";
        private const string CHANGE_PASSWORD_ENDPOINT = "accounts/change-password";

        private readonly IHttpClient httpClient;
        public HomeController(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public IActionResult Account()
        {
            if(httpClient.GetUserInfo() == null) 
            {
                HttpResponseMessage response = httpClient.GetAsync(USERINFO_ENDPOINT);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)  //sign aout user after sign in, because token isn't persistent and is not included in header
                {
                    return httpClient.GetLoginAction();
                }
            }
            AccountViewModel model = new AccountViewModel()
            {
                UserInfo = httpClient.GetUserInfo(),
                PasswordChange = new PasswordChangeModel()
            };

            return View(model);
        }

        [Authorize(Roles = SupportedRoles.OWN_USER_ACCOUNT_EDIT)]
        public IActionResult SubmitChangePassword(AccountViewModel model)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "oldPassword", model.PasswordChange.OldPassword },
                { "newPassword", model.PasswordChange.NewPassword },
            });
            HttpResponseMessage response = httpClient.PostAsync(CHANGE_PASSWORD_ENDPOINT, content);
            return RedirectToAction("Account");
        }
    }
}
