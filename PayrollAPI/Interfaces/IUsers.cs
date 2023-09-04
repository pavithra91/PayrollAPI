using Microsoft.AspNetCore.Mvc;
using PayrollAPI.DataModel;
using PayrollAPI.Models;
using PayrollAPI.Repository;

namespace PayrollAPI.Interfaces
{
    public interface IUsers
    {
        public TokenResponse AuthenticateUser(Users usr);

        public TokenResponse RefreshToken(TokenResponse tokenResponse);

        public User GetUser(string username);
        public bool CreateUser(UserDto user);

        public bool ResetPassword(string username, string password);
    }
}
