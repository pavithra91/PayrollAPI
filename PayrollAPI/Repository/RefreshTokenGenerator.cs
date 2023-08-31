using PayrollAPI.Data;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;
using System.Security.Cryptography;

namespace PayrollAPI.Repository
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        private readonly DBConnect _context;

        public RefreshTokenGenerator(DBConnect context)
        {
            _context = context;
        }
        public string GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string RefreshToken = Convert.ToBase64String(randomnumber);

                var _user = _context.LoginInfo.FirstOrDefault(o => o.userID == username);
                if (_user != null)
                {
                    _user.refreshToken = RefreshToken;
                    _context.SaveChanges();
                }
                else
                {
                    LoginInfo tblRefreshtoken = new LoginInfo()
                    {
                        userID = username,
                        tokenID = new Random().Next().ToString(),
                        refreshToken = RefreshToken,
                        isActive = true
                    };
                }

                return RefreshToken;
            }
        }
    }
}
