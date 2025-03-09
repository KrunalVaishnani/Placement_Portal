using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Placement_Portal_APIs.Data;
using Placement_Portal_APIs.Models; // Make sure this namespace contains your UserModel
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Placement_Portal_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserRepository _userRepository;
        public AuthController(IConfiguration config, UserRepository userRepository)
        {
            _config = config;
            _userRepository = userRepository;
        }

        // POST: api/Auth/login
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

                    // Generate JWT Token
                    var token = GenerateJwtToken(user.UserName.ToString(), user.UserID.ToString());

                    return Ok(new
                    {
                        Message = "Login successful",
                        User = user,
                        Token = token
                    });
                }
                else
                {
                    return Unauthorized("Invalid username or password.");
                }
            }

            return BadRequest("Invalid data.");
        }

        private string GenerateJwtToken(string username, string userId)
        {
            string key = _config["Jwt:Key"];

            if (string.IsNullOrEmpty(key) || key.Length < 32)
            {
                throw new ArgumentException("JWT Key must be at least 32 characters long.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("UserID", userId)
    };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
