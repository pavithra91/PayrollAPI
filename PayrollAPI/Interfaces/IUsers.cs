using Microsoft.AspNetCore.Mvc;
using PayrollAPI.DataModel;
using PayrollAPI.Models;
using PayrollAPI.Repository;

namespace PayrollAPI.Interfaces
{
    public interface IUsers
    {
        public string AuthenticateUser(Users usr);

        public User GetUser(string username);
        public bool CreateUser(UserDto user);
    }
}
