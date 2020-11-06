using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AccessPointControlClient.DTO;
using AccessPointControlClient.HttpClientHelpers;
using AccessPointControlClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccessPointControlClient.Controllers
{
    [Authorize(Roles = SupportedRoles.USER_ACCOUNT_MANAGEMENT)]
    public class UserController : Controller
    {
        private const string REGISTER_ENDPOINT = "accounts/register";
        private const string USER_ACCOUNTS_ENDPOINT = "accounts/users";
        private const string USER_ACCOUNT_DELETE_ENDPOINT = "accounts/delete/{id}";
        private const string USER_ACCOUNT_UPDATE_ENDPOINT = "accounts/update";

        private readonly IHttpClient httpClient;
        public UserController(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        [Authorize(Roles = SupportedRoles.USER_ACCOUNT_MANAGEMENT)]
        public IActionResult Users()
        {
            UsersModel usersModel = new UsersModel()
            {
                SelectedUser = new UserDetailsModel(httpClient.GetUserInfo()),
                Users = GetAllUsers(),
                UserRegistrationModel = new UserRegistrationModel()
            };
            return View(usersModel);
        }

        public IActionResult RegisterUser(UsersModel model)
        {
            var roles = model.UserRegistrationModel.RoleValues.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();
            UserRegistrationModel regModel = model.UserRegistrationModel;
            RegistrationInfoDTO content = new RegistrationInfoDTO(regModel.Username, roles, regModel.Password);
            HttpResponseMessage response = httpClient.PostAsync(REGISTER_ENDPOINT, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
            List<UserDetailsModel> users = GetAllUsers();
            UserDetailsModel newUser = users.FirstOrDefault(u => u.Username == regModel.Username);
            UsersModel usersModel = new UsersModel()
            {
                SelectedUser = newUser,
                Users = users,
                UserRegistrationModel = new UserRegistrationModel()
            };

            return View(nameof(Users), usersModel);
        }

        [HttpPost]
        public IActionResult SelectUser(IFormCollection form)
        {
            List<UserDetailsModel> users = GetAllUsers();
            List<UserDetailsModel> userDetails = users.Select(u => new UserDetailsModel(u)).ToList();

            string selectedUserId = form["selectedUserId"];
            UserDetailsModel selectedUser = userDetails.FirstOrDefault(u => u.Id == selectedUserId);
            UsersModel usersModel = new UsersModel()
            {
                SelectedUser = selectedUser,
                Users = userDetails,
                UserRegistrationModel = new UserRegistrationModel()
            };
            return View(nameof(Users), usersModel);
        }

        [HttpPost]
        public IActionResult DeleteUser(IFormCollection form)
        {
            string selectedUserId = form["selectedUserId"];
            string newSuffix = USER_ACCOUNT_DELETE_ENDPOINT.Replace("{id}", selectedUserId);
            if (selectedUserId != httpClient.GetUserInfo().Id)
            {
                HttpResponseMessage response = httpClient.DeleteAsync(newSuffix);
            }
            return RedirectToAction(nameof(Users));
        }

        public IActionResult UpdateUser(UsersModel model)
        {
            var roles = model.SelectedUser.RoleValues.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();
            UserInfo user = new UserInfo(model.SelectedUser.Id, model.SelectedUser.Username, roles);
            HttpResponseMessage response = httpClient.PostAsync(USER_ACCOUNT_UPDATE_ENDPOINT,
                new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            List<UserDetailsModel> users = GetAllUsers();
            UserDetailsModel updatedUser = users.FirstOrDefault(u => u.Id == user.Id);
            UsersModel usersModel = new UsersModel()
            {
                SelectedUser = updatedUser,
                Users = users,
                UserRegistrationModel = new UserRegistrationModel()
            };

            return View(nameof(Users), usersModel);
        }

        private List<UserDetailsModel> GetAllUsers()
        {
            HttpResponseMessage response = httpClient.GetAsync(USER_ACCOUNTS_ENDPOINT);
            List<UserInfo> users = new List<UserInfo>();
            users = JsonConvert.DeserializeAnonymousType(response.Content.ReadAsStringAsync().Result, users);
            return users.Select(u => new UserDetailsModel(u)).OrderBy(u => u.Username).ToList();
        }
    }
}
