namespace PayrollAPI.DataModel
{
    public class PwdResetDto
    {
        public string? currentPassword { get; set; }
        public string password { get; set; }
        public string lastUpdateBy { get; set; }
    }
}
