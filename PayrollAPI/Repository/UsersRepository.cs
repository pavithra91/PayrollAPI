using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PayrollAPI.Repository
{
    public class UsersRepository : IUsers
    {
        private readonly DBConnect _dbConnect;
        private readonly JWTSetting setting;
        private readonly PasswordHasher passwordHasher;
        public UsersRepository(DBConnect dB, IOptions<JWTSetting> options) 
        {
            _dbConnect = dB;
            this.setting = options.Value;
            passwordHasher = new PasswordHasher();
        }

        public User GetUser(string username)
        {
            return null;
            //var _user = _dbConnect.User.FirstOrDefault(o => o.userID == username)
        }

        public bool CreateUser(UserDto user) 
        {
            string pwdHash = passwordHasher.Hash(user.password, user.epf.ToString(), user.costCenter);
            var _user = new User
            {
                userID = user.userID,
                epf = user.epf,
                empName = user.empName,
                costCenter = user.costCenter,
                role = user.role,
                pwdHash = pwdHash,
                status = true,
                createdBy = user.createdBy,
                createdDate = DateTime.Now
            };

            _dbConnect.Add(_user);
            return Save();
        }

        public string AuthenticateUser(Users usr)
        {
            var _user = _dbConnect.User.FirstOrDefault(o => o.userID == usr.Name && o.pwdHash == usr.Password);

            // if (_user == null)
            // return Unauthorized();

            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(setting.securitykey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "Pavithra"),
                    }
                ),
                Expires = DateTime.Now.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenhandler.CreateToken(tokenDescriptor);
            string finaltoken = tokenhandler.WriteToken(token);

            return finaltoken;
        }

        public bool Save()
        {
            var saved = _dbConnect.SaveChanges();
            return saved > 0 ? true: false;
        }
    }
}
