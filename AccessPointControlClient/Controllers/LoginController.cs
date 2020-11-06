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
            httpClient.Logout();
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            ViewData["Title"] = "Access point control system";
            return View(new LoginModel());
        }

        public IActionResult SubmitLoginData(LoginModel loginModel)
        {
           httpClient.Login(loginModel);
           return httpClient.GetDefaultAction();
        }
    }
}
