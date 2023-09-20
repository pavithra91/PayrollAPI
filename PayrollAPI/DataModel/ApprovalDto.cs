namespace PayrollAPI.DataModel
{
    public class ApprovalDto
    {
        public int period { get; set; }
        public int companyCode { get; set; }
        public string? approvedBy { get; set; }
    }
}
