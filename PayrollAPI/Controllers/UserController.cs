using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;
using PayrollAPI.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PayrollAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsers _usr;
        public UserController(IUsers users) 
        {
             _usr = users;
        }

        [AllowAnonymous]
        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] Users usr)
        {
            var _user = _usr.AuthenticateUser(usr);
            if (_user != null)
            {
                return Ok(_user); 
            }
            else
            {
                return Unauthorized();
            }
            
        }

        [Route("Refresh")]
        [HttpPost]
        public IActionResult Refresh([FromBody] TokenResponse token)
        {
            var _refreshToken = _usr.RefreshToken(token);
            if(_refreshToken == null)
            {
                return Unauthorized();
            }
            else
            {
                return Ok(_refreshToken);
            }
        }

        [Route("CreateUser")]
        [HttpPost]
        public IActionResult CreateUser([FromBody] UserDto usrDto)
        {
            bool _user = _usr.CreateUser(usrDto);

            if(_user)
                return Ok(_user);
            else 
                return BadRequest();
        }

        [Route("ResetPassword")]
        [HttpPost]
        public IActionResult ResetPassword([FromBody] UserDto usrDto)
        {
            bool _user = _usr.ResetPassword(usrDto.userID, usrDto.password);

            if (_user)
                return Ok(_user);
            else
                return BadRequest();
        }


        [Route("GetUser")]
        [HttpGet]
        public IActionResult GetUser()
        {
            return Ok("Its Working");
        }
    }
}
