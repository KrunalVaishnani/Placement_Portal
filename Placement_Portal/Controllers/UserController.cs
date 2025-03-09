using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Placement_Portal.Models;
using System.Net.Http.Headers;

namespace Placement_Portal.Controllers
{
    
    public class UserController : Controller
    {
        private readonly Uri baseAddress = new Uri("http://localhost:5075/api/");
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _client = new HttpClient { BaseAddress = baseAddress };
            _configuration = configuration;
        }

        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("token");

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                Console.WriteLine("⚠ No JWT Token found in session.");
            }
        }


        // GET: Retrieve all users

        [HttpGet]
        public IActionResult UserList()
        {
            List<UserModel> users = new List<UserModel>();
            HttpResponseMessage response = _client.GetAsync("User/GetAllUsers").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<UserModel>>(data);
            }
            else
            {
                ModelState.AddModelError("", "Error fetching user data.");
            }

            return View("UserList", users);
        }

        // POST: Delete a user
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            HttpResponseMessage response = _client.DeleteAsync($"User/DeleteUser/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting user.";
            }

            return RedirectToAction("UserList");
        }

        // GET: Create user view
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: Create a new user
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync("User/Add", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("UserList");
                }
            }

            TempData["ErrorMessage"] = "Error creating user. Please check the details.";
            return View(model);
        }

        // GET: Edit user view
        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            UserModel user = new UserModel();
            HttpResponseMessage response = await _client.GetAsync($"User/GetUserByID/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<UserModel>(data);
            }

            return View(user);
        }

        // POST: Update user
        [HttpPost]
        public async Task<IActionResult> EditUser(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PutAsync("User/Update", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("UserList");
                }
            }

            TempData["ErrorMessage"] = "Error updating user. Please check the details.";
            return View(model);
        }

        // GET: Login user view
        public IActionResult UserLogin()
        {
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync($"{_client.BaseAddress}Auth", content);

                string responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response: " + responseData); // Log API response

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<LoginResponse>(data);
                    if (result != null && result.User != null)
                    {
                        Console.WriteLine("seaaion");
                        HttpContext.Session.SetString("UserId", result.User.UserID.ToString());
                        HttpContext.Session.SetString("UserName", result.User.UserName?.ToString() ?? "");
                        HttpContext.Session.SetString("Role", result.User.Role?.ToString() ?? "");
                        HttpContext.Session.SetString("token", result.token);

                        //return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login response format.");
                        return View(model);
                    }
                    if (HttpContext.Session.GetString("Role") == "Admin")
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    else if (HttpContext.Session.GetString("Role") == "Student")
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Student" });
                    }
                    var ErrorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(ErrorMessage);
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid credentials or role!";
                    return View("UserLogin");
                }

                ModelState.AddModelError("", responseData);
                return View(model);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error connecting to the API: " + ex.Message);
                return View(model);
            }
        }



        // GET: Auth/Register
        public IActionResult UserRegister()
        {
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        public async Task<IActionResult> UserRegister(UserRegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync($"User/Register", content);

                var responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Raw API Response: " + responseData); // Debugging

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseData, new JsonSerializerSettings
                        {
                            ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                            {
                                NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy()
                            }
                        });

                        Console.WriteLine("Deserialized Result: " + JsonConvert.SerializeObject(result)); // Debugging

                        if (result != null && result.ContainsKey("message"))
                        {
                            TempData["SuccessMessage"] = result["message"].ToString();
                            return RedirectToAction("UserLogin");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Unexpected response format from server.");
                            Console.WriteLine("Response does not contain 'message' field.");
                        }
                    }
                    catch (JsonException ex)
                    {
                        ModelState.AddModelError(string.Empty, "Error processing API response.");
                        Console.WriteLine("JSON Deserialization Error: " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("API Error Response: " + responseData); // Debugging for API failure cases

                    if (!string.IsNullOrEmpty(responseData))
                    {
                        try
                        {
                            var errorResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseData);

                            if (errorResult != null && errorResult.ContainsKey("message"))
                            {
                                ModelState.AddModelError(string.Empty, errorResult["message"].ToString());
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "An unknown error occurred. Please try again.");
                            }
                        }
                        catch (JsonException)
                        {
                            ModelState.AddModelError(string.Empty, "Error processing API response.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No response from server.");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, "Error connecting to the API: " + ex.Message);
                Console.WriteLine("HTTP Request Exception: " + ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred: " + ex.Message);
                Console.WriteLine("General Exception: " + ex.Message);
            }

            return View(model);
        }

        // POST: Auth/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
