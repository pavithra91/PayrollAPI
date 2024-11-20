using Microsoft.AspNetCore.Mvc;
using PayrollAPI.DataModel;
using PayrollAPI.Models;
using PayrollAPI.Repository;

namespace PayrollAPI.Interfaces
{
    public interface IUsers
    {
        public TokenResponse? AuthenticateUser(Users usr, out string msg, out int status);

        public TokenResponse RefreshToken(TokenResponse tokenResponse);

        public Task<MsgDto> GetUsers();
        
        public Task<MsgDto> GetUserbyId(int id);
        public Task<MsgDto> GetUserbyCostCenter(string costCenter);

        public Task<MsgDto> CreateUser(UserDto user);

        public Task<MsgDto> UpdateUser(UserDto user);

        public Task<MsgDto> DeleteUser(UserDto userDto);

        public bool ResetPassword(string username, string password);

        public Task<MsgDto> SignOut(string userID);
    }
}
