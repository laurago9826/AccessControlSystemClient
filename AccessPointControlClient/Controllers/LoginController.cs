using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AccessPointControlClient.HttpClientHelpers;
using AccessPointControlClient.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccessPointControlClient.Controllers
{
    public class LoginController : Controller
    {
        private IHttpClient httpClient;
        public LoginController(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public IActionResult Logout()
        {
            httpClient.Logout(HttpContext);
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        public IActionResult SubmitLoginData(LoginModel loginModel)
        {
           httpClient.Login(loginModel, HttpContext);
            return httpClient.GetDefaultAction();

        }
    }
}
