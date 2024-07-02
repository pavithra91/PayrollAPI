namespace PayrollAPI.DataModel
{
    public class PayCodeDto
    {
        public int id { get; set; }
        public int companyCode { get; set; }
        public int payCode { get; set; }
        public string? calCode { get; set; }
        public string? payCategory { get; set; }
        public string? description { get; set; }
        public string? taxationType { get; set; }
        public decimal rate { get; set; }
        public string? createdBy { get; set; }
        public DateTime createdDate { get; set; }
        public string? lastUpdateBy { get; set; }
        public DateTime lastUpdateDate { get; set; }
    }
}
