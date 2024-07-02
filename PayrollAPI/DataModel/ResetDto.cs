namespace PayrollAPI.DataModel
{
    public class ResetDto
    {
        public int period { get; set; }
        public int companyCode { get; set; }
        public bool resetData { get; set; }
        public bool resetTempData { get; set; }
        public string? userID { get; set; }
    }
}
