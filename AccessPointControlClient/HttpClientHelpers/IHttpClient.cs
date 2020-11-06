using AccessPointControlClient.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AccessPointControlClient.HttpClientHelpers
{
    public interface IHttpClient
    {
        IActionResult GetDefaultAction();
        IActionResult GetLoginAction();

        UserInfo GetUserInfo();
        string GetServerBaseUrl();
        bool Login(LoginModel loginModel);
        void Logout();
        HttpResponseMessage PostAsync(string endPointSuffixUri, HttpContent content);
        HttpResponseMessage GetAsync(string endPointSuffixUri);

        HttpResponseMessage DeleteAsync(string endPointSuffixUri);
    }
}
