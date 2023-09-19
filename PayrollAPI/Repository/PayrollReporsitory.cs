using Microsoft.EntityFrameworkCore;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;

namespace PayrollAPI.Repository
{
    public class PayrollReporsitory : IPayroll
    {
        private readonly DBConnect _dbConnect;

        public PayrollReporsitory(DBConnect db)
        {
            _dbConnect = db;
        }

        public void ProcessPayroll(ApprovalDto approvalDto)
        {
            ICollection<Employee_Data> _emp = _dbConnect.Employee_Data.Where(o => o.period == approvalDto.period).ToList();
            ICollection<Payroll_Data> _payrollData = _dbConnect.Payroll_Data.Where(o => o.period == approvalDto.period).ToList();           
            ICollection<EmpSpecialRate> _empSpecialRates = _dbConnect.EmpSpecialRate.Where(o => o.status == true).ToList();


        }
    }
}
