using System;
using System.Text;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using helloworld.Helpers;
using helloworld.Services;
using helloworld.DTOs;
using helloworld.Entities;
namespace helloworld.Controllers 
{

    [Route("api/[controller]")]
    public class UsersController : Controller {
        private IUserService _userService;
        private readonly AppSettings _appSettings;
        public UsersController(IUserService userService, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserDTO userDTO)
        {
            var user = _userService.Authenticate(userDTO.Email, userDTO.Password);

            if (user == null)
                return BadRequest(new { message = "Email или пароль неверны." });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info (without password) and token to store client side
            return Ok(new {
                Id = user.Id,
                Email = user.Email,
                Token = tokenString
            });
        }

        [HttpPost]
        public IActionResult Register([FromBody]UserDTO userDTO)
        {
            // map dto to entity
            var user = new User() {
                Email = userDTO.Email
            }; 

            try 
            {
                // save 
                _userService.Create(user, userDTO.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
         
    }
}