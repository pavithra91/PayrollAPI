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

        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] Users usr)
        {
            var _user = _usr.AuthenticateUser(usr);
            return Ok(_user);
        }

        [Route("CreateUser")]
        [HttpPost]
        public IActionResult CreateUser([FromBody] UserDto usr)
        {
            bool _user = _usr.CreateUser(usr);

            if(_user)
                return Ok(_user);
            else 
                return BadRequest();
        }
    }
}
