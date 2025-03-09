using Microsoft.AspNetCore.Mvc;
using Placement_Portal_APIs.Data;
using Placement_Portal_APIs.Models;
using System.Data;

namespace Placement_Portal_APIs.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userRepository.SelectAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _userRepository.SelectByPk(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }

        [HttpPost]
        public IActionResult AddUser(UserModel user)
        {
            var isAdded = _userRepository.Add(user);
            if (!isAdded)
            {
                return BadRequest("Failed to add user.");
            }
            return Ok("User added successfully.");
        }

        [HttpPut]
        public IActionResult UpdateUser(UserModel user)
        {
            var isUpdated = _userRepository.Update(user);
            if (!isUpdated)
            {
                return BadRequest("Failed to update user.");
            }
            return Ok("User updated successfully.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var isDeleted = _userRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound("User not found.");
            }
            return NoContent();
        }

        [HttpPost]
        public IActionResult Login(UserLoginModel userLoginModel)
        {
            if (ModelState.IsValid)
            {
                DataTable userTable = _userRepository.LoginUser(userLoginModel.UserName, userLoginModel.Password, userLoginModel.Role);

                if (userTable.Rows.Count > 0)
                {
                    var userRow = userTable.Rows[0];
                    var user = new
                    {
                        UserID = userRow["UserID"],
                        UserName = userRow["UserName"],
                        Role = userRow["Role"]
                    };

                    return Ok(new
                    {
                        Message = "Login successful",
                        User = user
                    });
                }
                else
                {
                    return Unauthorized("Invalid username or password.");
                }
            }

            return BadRequest("Invalid data.");
        }

        [HttpPost]
        public IActionResult Register(UserRegisterModel userRegisterModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DataTable resultTable = _userRepository.RegisterUser(
                        userRegisterModel.UserName,
                        userRegisterModel.Password,
                        userRegisterModel.Email,
                        userRegisterModel.Contact_No,
                        userRegisterModel.Role
                    );

                    if (resultTable.Rows.Count > 0)
                    {
                        var resultRow = resultTable.Rows[0];
                        var resultMessage = resultRow["Message"].ToString();

                        if (resultMessage == "User registered successfully")
                        {
                            return Ok(new { Message = resultMessage });
                        }
                        else
                        {
                            return Conflict(new { Message = resultMessage });
                        }
                    }
                    else
                    {
                        return StatusCode(500, "User registration failed. No response from the database.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                    return StatusCode(500, "An unexpected error occurred: " + ex.Message);
                }
            }

            return BadRequest("Invalid data.");
        }

        [HttpGet]
        public IActionResult GetTop10Users()
        {
            var top10Users = _userRepository.GetTop10Users();
            return Ok(top10Users);
        }

        [HttpGet]
        public IActionResult GetUserCount()
        {
            var userCount = _userRepository.GetUserCount();
            return Ok(userCount);
        }
    }
}
