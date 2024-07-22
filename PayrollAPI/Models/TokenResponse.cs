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
        public int ID { get; set; }

        public string EPF { get; set; }
        public string EmpName { get; set; }
        public string UserID { get; set; }
        public string CostCenter { get; set; }
        public string Role { get; set; }

        public UserDetails(int _id, string _epf, string _empName, string _userID, string _costCenter, string _role)
        {
            ID = _id;
            EPF = _epf;
            EmpName = _empName;
            UserID = _userID;
            CostCenter = _costCenter;
            Role = _role;
        }
    }
}
