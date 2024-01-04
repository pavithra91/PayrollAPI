using LinqToDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;
using System.Data;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PayrollAPI.Repository
{
    public class UsersRepository : IUsers
    {
        private readonly DBConnect _context;
        private readonly JWTSetting setting;
        private readonly PasswordHasher passwordHasher;
        private readonly IRefreshTokenGenerator tokenGenerator;
        public UsersRepository(DBConnect dB, IOptions<JWTSetting> options, IRefreshTokenGenerator _refreshToken) 
        {
            _context = dB;
            this.setting = options.Value;
            passwordHasher = new PasswordHasher();
            tokenGenerator = _refreshToken;
        }

        public async Task<MsgDto> GetUsers()
        {
            MsgDto _msg = new MsgDto();
            try
            {                
                ICollection<User> _userList = await _context.User.ToListAsync();

                if (_userList.Count > 0)
                {
                    _msg.Data = JsonConvert.SerializeObject(_userList);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> GetUserbyId(int id)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                User _user = await _context.User.Where(o=>o.id == id).FirstOrDefaultAsync();

                if (_user != null)
                {
                    _msg.Data = JsonConvert.SerializeObject(_user);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> CreateUser(UserDto user) 
        {
            try
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

                _context.Add(_user);

                MsgDto _msg = new MsgDto();
                _msg.MsgCode = 'S';
                _msg.Message = "User Created";
                return _msg;
            }
            catch(Exception ex)
            {
                MsgDto _msg = new MsgDto();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public TokenResponse AuthenticateUser(Users usr)
        {
            TokenResponse tokenResponse = new TokenResponse();

            var _user = _context.User.FirstOrDefault(o => o.userID == usr.UserId);

            bool pwd = passwordHasher.Verify(_user.pwdHash, usr.Password, _user.epf.ToString(), _user.costCenter);

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

            tokenResponse.JWTToken = finaltoken;
            tokenResponse.RefreshToken = tokenGenerator.GenerateToken(_user.userID);

            UserDetails _userDetails = new UserDetails();
            _userDetails.EPF = _user.epf;
            _userDetails.CostCenter = _user.costCenter;
            _userDetails.Role = _user.role;

            tokenResponse._userDetails = _userDetails;

            LoginInfo _log = new LoginInfo();
            _log.userID = _user.userID;
            _log.tokenID = tokenResponse.JWTToken.ToString();
            _log.refreshToken = tokenResponse.RefreshToken;
            _log.loginDateTime = DateTime.Now;
            _log.isActive = true;

            _context.LoginInfo.Add(_log);
            Save();

            return tokenResponse;
        }

        public TokenResponse RefreshToken(TokenResponse token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principle = tokenHandler.ValidateToken(token.JWTToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.securitykey)),
                ValidateIssuer = false,
                ValidateAudience = false
            }, out securityToken);

            var _token = securityToken as JwtSecurityToken;
            if (_token != null && _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256)) 
            {
                return null;
            }

            var username = principle.Identity.Name;
            var _reftable = _context.LoginInfo.FirstOrDefault(o => o.userID == username && o.refreshToken == token.RefreshToken);
            if(_reftable==null) 
            {
                return null;
            }

            TokenResponse _result = Authenticate(username, principle.Claims.ToArray());


            return _result;
        }

        public TokenResponse Authenticate(string username, Claim[] claims)
        {
            TokenResponse tokenResponse = new TokenResponse();
            var tokenkey = Encoding.UTF8.GetBytes(setting.securitykey);
            var tokenhandler = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                 signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)

                );
            tokenResponse.JWTToken = new JwtSecurityTokenHandler().WriteToken(tokenhandler);
            tokenResponse.RefreshToken = tokenGenerator.GenerateToken(username);

            return tokenResponse;
        }

        public bool ResetPassword(string username, string password)
        {
            var _user = _context.User.FirstOrDefault(o => o.userID == username);

            string pwdHash = passwordHasher.Hash(password, _user.epf.ToString(), _user.costCenter);

            _user.pwdHash = pwdHash;


            _context.Update(_user);
            return Save();
        }


        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true: false;
        }
    }
}
