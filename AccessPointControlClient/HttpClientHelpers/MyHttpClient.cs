using AccessPointControlClient.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AccessPointControlClient.HttpClientHelpers
{
    public class MyHttpClient : IHttpClient
    {
        private readonly IActionResult loginAction = new RedirectToActionResult("Login", "Login", null);
        private readonly IActionResult defaultAction = new RedirectToActionResult("Index", "Home", null);
        private HttpClient httpClient = new HttpClient();
        private UserInfo userInfo;
        private string serverBaseUrl;

        private const string LOGIN_ENDPOINT = "login";
        private const string USERINFO_ENDPOINT = "accounts/user-info";

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

        private void SetServerBaseUrl(string ip, int port)
        {
            this.serverBaseUrl = "http://" + ip + ":" + port + "/api/";
        }

        public void AddTokenToHeader(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public bool Login(LoginModel loginModel, HttpContext httpContext)
        {
            SetServerBaseUrl(loginModel.ServerIP, loginModel.Port);
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

            //GET USER INFO
            response = GetAsync(USERINFO_ENDPOINT);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }
            this.userInfo = JsonConvert.DeserializeAnonymousType(response.Content.ReadAsStringAsync().Result, this.userInfo);

            //BUILD CLAIMS PRINCIPAL
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userInfo.Username));
            userInfo.Roles.ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r)));
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            DateTimeOffset currentUtc = new SystemClock().UtcNow;
            httpContext.SignInAsync(principal, new AuthenticationProperties()
            { ExpiresUtc = currentUtc.AddSeconds(expiresIn)});
            return true;
        }

        public void Logout(HttpContext httpContext)
        {
            this.userInfo = null;
            httpContext.SignOutAsync();
            this.httpClient.DefaultRequestHeaders.Authorization = null;
        }


        public bool IsUserLoggedIn()
        {
            return userInfo != null;
        }


        public HttpResponseMessage PostAsync(string endPointSuffixUri, HttpContent content)
        {
            return httpClient.PostAsync(new Uri(GetServerBaseUrl() + endPointSuffixUri), content).Result;
        }

        public HttpResponseMessage GetAsync(string endPointSuffixUri)
        {
            return httpClient.GetAsync(new Uri(GetServerBaseUrl() + endPointSuffixUri)).Result;
        }
    }
}
