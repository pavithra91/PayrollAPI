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

        /// <summary>
        /// Method used to authenticate API requet User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] Users user)
        {
            var _user = _usr.AuthenticateUser(user);
            if (_user != null)
            {
                return Ok(_user); 
            }
            else
            {
                return Unauthorized();
            }
            
        }
        /// <summary>
        /// Method used to refresh the JWT token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Method used to Create new User
        /// </summary>
        /// <param name="usrDto"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Method used to Reset existing User's Password
        /// </summary>
        /// <param name="usrDto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Used to get all users in the system
        /// </summary>
        /// <returns></returns>
        [Route("GetUsers")]
        [HttpGet]
        public IActionResult GetUsers()
        {
            ICollection<User> _userList = _usr.GetUsers();

            if (_userList != null)
                return Ok(_userList);
            else
                return BadRequest();
        }
    }
}
