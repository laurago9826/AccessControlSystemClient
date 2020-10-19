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

        bool IsUserLoggedIn();
        string GetServerBaseUrl();
        bool Login(LoginModel loginModel, HttpContext httpContext);
        void Logout(HttpContext httpContext);
        HttpResponseMessage PostAsync(string endPointSuffixUri, HttpContent content);
        HttpResponseMessage GetAsync(string endPointSuffixUri);
    }
}
