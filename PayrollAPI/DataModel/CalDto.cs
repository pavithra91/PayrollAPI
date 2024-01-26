namespace PayrollAPI.DataModel
{
    public class CalDto
    {
        public int id { get; set; }
        public char flag { get; set; }
        public int companyCode { get; set; }
        public int sequence { get; set; }
        public int payCode { get; set; }
        public string? calCode { get; set; }
        public string? payCategory { get; set; }
        public string? calFormula { get; set; }
        public string? calDescription { get; set; }
        public string? contributor { get; set; }
        public bool status { get; set; }
        public string? createdBy { get; set; }
        public DateTime createdDate { get; set; }
        public string? lastUpdateBy { get; set; }
        public DateTime? lastUpdateDate { get; set; }
    }
}
