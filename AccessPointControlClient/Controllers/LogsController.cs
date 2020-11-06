using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AccessPointControlClient.DTO;
using AccessPointControlClient.HttpClientHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccessPointControlClient.Controllers
{
    [Authorize(Roles = SupportedRoles.LOG_ACCESS)]
    public class LogsController : Controller
    {
        private const string LOGS_ENDPOINT = "logs";

        private static LogsDTO globalLogsInstance = new LogsDTO();

        private readonly IHttpClient httpClient;
        public LogsController(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public IActionResult Logs()
        {
            HttpResponseMessage response = httpClient.GetAsync(LOGS_ENDPOINT);
            LogsDTO logsDTO = null;
            logsDTO = JsonConvert.DeserializeAnonymousType(response.Content.ReadAsStringAsync().Result, logsDTO);
            logsDTO.LogContent.ForEach(l => l.FileContent.Replace(Environment.NewLine, "<br />"));
            logsDTO.SetSelectedEntry(logsDTO.LogContent.FirstOrDefault());
            globalLogsInstance = logsDTO;
            return View(logsDTO);
        }

        public IActionResult SelectLogFile(IFormCollection form)
        {
            string logFile = form["logFile"];
            LogEntry selectedLogEntry = globalLogsInstance.LogContent.FirstOrDefault(l => l.FileName == logFile);
            globalLogsInstance.SetSelectedEntry(selectedLogEntry);
            return View(nameof(Logs), globalLogsInstance);
        }
    }
}
