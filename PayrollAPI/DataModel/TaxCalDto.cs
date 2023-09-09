namespace PayrollAPI.DataModel
{
    public class TaxCalDto
    {
        public int id { get; set; }
        public char flag { get; set; }
        public decimal range { get; set; }
        public string? calFormula { get; set; }
        public string? description { get; set; }
        public bool status { get; set; }
        public string? createdBy { get; set; }
        public DateTime createdDate { get; set; }
        public string? lastUpdateBy { get; set; }
        public DateTime lastUpdateDate { get; set; }
    }
}
