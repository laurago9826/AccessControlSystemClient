using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AccessPointControlClient.DTO;
using AccessPointControlClient.HttpClientHelpers;
using AccessPointControlClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccessPointControlClient.Controllers
{
    [Authorize(Roles = SupportedRoles.VIEW_DEBUG)]
    public class DebugController : Controller
    {
        private const string DEBUG_INFO_ENDPOINT = "camera-client/evaluation-info";

        private readonly IHttpClient httpClient;
        public DebugController(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public IActionResult Debug()
        {
            HttpResponseMessage response = httpClient.GetAsync(DEBUG_INFO_ENDPOINT);
            DebugInfoDTO info = null;
            info = JsonConvert.DeserializeAnonymousType(response.Content.ReadAsStringAsync().Result, info);
            if (info == null || info.Info == null)
            {
                return View(DebugInfo.CreateEmptyDebugInfo());
            }
            return View(new DebugInfo(info));
        }
    }
}
