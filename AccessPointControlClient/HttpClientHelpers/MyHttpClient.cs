using AccessPointControlClient.Models;
using ICSharpCode.Decompiler.Util;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Resources;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AccessPointControlClient.HttpClientHelpers
{

    public class MyHttpClient : IHttpClient
    {
        private IHttpContextAccessor httpContextAccessor;

        private readonly IActionResult loginAction = new RedirectToActionResult("Login", "Login", null);
        private readonly IActionResult defaultAction = new RedirectToActionResult("Account", "Home", null);
        private HttpClient httpClient = new HttpClient();
        private UserInfo userInfo;
        private string serverBaseUrl;

        private const string LOGIN_ENDPOINT = "login";
        private const string USERINFO_ENDPOINT = "accounts/user-info";

        public MyHttpClient(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            httpContextAccessor.HttpContext.SignOutAsync(); //sign aout user after sign in, because token isn't persistent and is not included in header

            httpClient.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

            Dictionary<string, string> data = ReadPropertiesFile("properties.txt");
            data.TryGetValue("server", out string server);
            data.TryGetValue("port", out string port);
            SetServerBaseUrl(server, int.Parse(port), false);

        }
        public IActionResult GetDefaultAction()
        {
            return defaultAction;
        }

        public IActionResult GetLoginAction()
        {
            return loginAction;
        }

        public string GetServerBaseUrl()
        {
            return serverBaseUrl;
        }

        private void SetServerBaseUrl(string ip, int port, bool updateProperties)
        {
            this.serverBaseUrl = "http://" + ip + ":" + port + "/api/";
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "server", ip },
                { "port", port.ToString() }
            };
            if(updateProperties)
                WriteToPropertiesFile("properties.txt", data);
        }

        public void AddTokenToHeader(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public bool Login(LoginModel loginModel)
        {
            SetServerBaseUrl(loginModel.ServerIP, loginModel.Port, true);
            //GET TOKEN
            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "username", loginModel.Username },
                { "password", loginModel.Password },
                { "grant_type", "password" },
            });
            HttpResponseMessage response = PostAsync(LOGIN_ENDPOINT, content);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }

            Dictionary<string, dynamic> keyValuePairs =
                JsonConvert.DeserializeAnonymousType(response.Content.ReadAsStringAsync().Result, new Dictionary<string, dynamic>());
            if (!keyValuePairs.TryGetValue("access_token", out dynamic token)
                || !keyValuePairs.TryGetValue("expires_in", out dynamic expiresIn))
            {
                return false;
            }
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpContextAccessor.HttpContext.Request.Headers.Add(new KeyValuePair<string, StringValues>("Authorization", "Bearer " + token));

            //GET USER INFO
            response = GetAsync(USERINFO_ENDPOINT);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }
            this.userInfo = JsonConvert.DeserializeAnonymousType(response.Content.ReadAsStringAsync().Result, this.userInfo);

            //CREATE CLAIMS PRINCIPAL
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userInfo.Username));
            userInfo.Roles.ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r)));
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            DateTimeOffset currentUtc = new SystemClock().UtcNow;
            httpContextAccessor.HttpContext.SignInAsync(principal, new AuthenticationProperties()
            { ExpiresUtc = currentUtc.AddSeconds(expiresIn) });
            
            return true;
        }

        public void Logout()
        {
            this.userInfo = null;
            httpContextAccessor.HttpContext.SignOutAsync();
            this.httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public HttpResponseMessage PostAsync(string endPointSuffixUri, HttpContent content)
        {
            return httpClient.PostAsync(new Uri(GetServerBaseUrl() + endPointSuffixUri), content).Result;
        }

        public HttpResponseMessage GetAsync(string endPointSuffixUri)
        {
            return httpClient.GetAsync(new Uri(GetServerBaseUrl() + endPointSuffixUri)).Result;
        }

        public HttpResponseMessage DeleteAsync(string endPointSuffixUri)
        {
            return httpClient.DeleteAsync(new Uri(GetServerBaseUrl() + endPointSuffixUri)).Result;
        }

        private static Dictionary<string, string> ReadPropertiesFile(string path)
        {
            var data = new Dictionary<string, string>();
            foreach (var row in File.ReadAllLines(path))
                data.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
            return data;
        }

        private static void WriteToPropertiesFile(string path, Dictionary<string, string> data)
        {
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                foreach (var d in data)
                {
                    sw.WriteLine(d.Key + "=" + d.Value);
                }
            }

        }

        public UserInfo GetUserInfo()
        {
            return userInfo;
        }
    }
}
