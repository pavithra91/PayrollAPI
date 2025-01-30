namespace PayrollAPI.DataModel
{
    public class StopSalDto
    {
        public int period { get; set; }
        public int companyCode { get; set; }
        public string? deleteBy { get; set; }
        public int[] id { get; set; }
    }
}
