using PayrollAPI.DataModel;

namespace PayrollAPI.Models
{
    public class TokenResponse
    {
        public string JWTToken { get; set; }
        public string RefreshToken { get; set; }
        public UserDetails _userDetails { get; set; }
    }

    public struct UserDetails
    {
        public string EPF { get; set; }
        public string CostCenter { get; set; }
        public string Role { get; set; }

        public UserDetails(string _epf, string _costCenter, string _role)
        {
            EPF = _epf;
            CostCenter = _costCenter;
            Role = _role;
        }
    }
}
