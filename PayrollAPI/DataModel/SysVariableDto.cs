namespace PayrollAPI.DataModel
{
    public class SysVariableDto
    {
        public int id { get; set; }
        public int companyCode { get; set; }
        public string category_name { get; set; }
        public string variable_name { get; set; }
        public string variable_value { get; set; }
        public string? createdBy { get; set; }
        public DateTime createdDate { get; set; }
        public string? lastUpdateBy { get; set; }
        public DateTime lastUpdateDate { get; set; }
    }
}
