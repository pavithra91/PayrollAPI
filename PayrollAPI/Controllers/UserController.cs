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
            var _user = _usr.AuthenticateUser(user, out string msg, out int status);

            if (_user != null && status == 1)
            {
                return Ok(_user); 
            }
            else if(status == -1)
            {
                return NotFound(msg);
            }
            else if (status == -2)
            {
                return Unauthorized(msg);
            }
            else
            {
                return BadRequest(msg);
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
        public async Task<IActionResult> CreateUser([FromBody] UserDto usrDto)
        {
            MsgDto _msg = await _usr.CreateUser(usrDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("update-user")]
        [HttpPut]
        public async Task<ActionResult> UpdateUser([FromBody] UserDto usrDto)
        {
            MsgDto _msg = await _usr.UpdateUser(usrDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else if (_msg.MsgCode == 'N')
            {
                return NotFound(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
        }

        [Route("delete-user")]
        [HttpPut]
        public async Task<ActionResult> DeleteUser([FromBody] UserDto usrDto)
        {
            MsgDto _msg = await _usr.DeleteUser(usrDto);

            if (_msg.MsgCode == 'S')
            {
                return Ok(_msg);
            }
            else if (_msg.MsgCode == 'N')
            {
                return NotFound(_msg);
            }
            else
            {
                return BadRequest(_msg);
            }
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
        public async Task<IActionResult> GetUsers()
        {
            MsgDto _msg = await _usr.GetUsers();

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }

        [Route("get-user-id")]
        [HttpGet]
        public async Task<IActionResult> GetUserById(int id)
        {
            MsgDto _msg = await _usr.GetUserbyId(id);

            if (_msg.MsgCode == 'S')
                return Ok(_msg);
            else
                return BadRequest(_msg);
        }
    }
}
