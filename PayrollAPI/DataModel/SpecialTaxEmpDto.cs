namespace PayrollAPI.DataModel
{
    public class SpecialTaxEmpDto
    {
        public int id { get; set; }
        public char flag { get; set; }
        public int companyCode { get; set; }
        public int epf { get; set; }
        public string? costcenter { get; set; }
        public string? calFormula { get; set; }
        public bool status { get; set; }
        public string? createdBy { get; set; }
        public string? lastUpdateBy { get; set; }
    }
}
