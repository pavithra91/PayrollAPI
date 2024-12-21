using Microsoft.EntityFrameworkCore;

namespace PayrollAPI.Models.Payroll
{
    [Keyless]
    public class Payroll_Summary_View
    {
        public int companyCode { get; set; }
        public int period { get; set; }
        public float Gross { get; set; }
        public int Employees { get; set; }
        public float EPFEMP { get; set; }
        public float EPFCOM { get; set; }
        public float ETF { get; set; }

    }
}
