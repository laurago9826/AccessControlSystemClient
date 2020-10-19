using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AccessPointControlClient.Models;
using System.Net.Http;
using AccessPointControlClient.HttpClientHelpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace AccessPointControlClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClient httpClient;
        private const string PEOPLE_ENDPOINT = "people/people";

        public HomeController(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles ="UserCreation")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            
            return View();
        }

        [Authorize(Roles = "blabla")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login", "LoginController");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
