namespace PayrollAPI.DataModel
{
    public class SpecialRateEmpDto
    {
        public int id { get; set; }
        public char flag { get; set; }
        public int companyCode { get; set; }
        public string epf { get; set; }
        public string? costCenter { get; set; }
        public int payCode { get; set; }
        public string? calCode { get; set; }
        public decimal rate { get; set; }
        public bool status { get; set; }
        public string? createdBy { get; set; }
        public string? lastUpdateBy { get; set; }
    }
}
