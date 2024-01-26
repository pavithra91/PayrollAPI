namespace PayrollAPI.DataModel
{
    public class EmpMasterDto
    {
        
        public string epf { get; set; }
        public int period { get; set; }
        public string empName { get; set; }
        public string costCenter { get; set; }
        public int gradeCode { get; set; }
        public string? empGrade { get; set; }
        public int paymentType { get; set; }
        public int bankCode { get; set; }
        public int branchCode { get; set; }
        public string? accountNo { get; set; }
        public string? createdBy { get; set; }

        
    }
}
