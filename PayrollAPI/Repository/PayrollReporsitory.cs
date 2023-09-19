using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace PayrollAPI.Repository
{
    public class PayrollReporsitory : IPayroll
    {
        private readonly DBConnect _context;

        public PayrollReporsitory(DBConnect db)
        {
            _context = db;
        }

        public void ProcessPayroll(ApprovalDto approvalDto)
        {
            ICollection<Employee_Data> _emp = _context.Employee_Data.Where(o => o.period == approvalDto.period).ToList();
            ICollection<Payroll_Data> _payrollData = _context.Payroll_Data.Where(o => o.period == approvalDto.period).ToList();
            ICollection<EmpSpecialRate> _empSpecialRates = _context.EmpSpecialRate.Where(o => o.status == true).ToList();

           // using var transaction = BeginTransaction();

            Parallel.ForEach(_empSpecialRates, payItem =>
            {
                Payroll_Data _data = new Payroll_Data();

                _data = _payrollData.Where(o=> o.epf == payItem.epf && o.payCode == payItem.payCode).FirstOrDefault();
                if(_data != null )
                {
                    _data.amount = payItem.rate;
                    _data.taxContribution = payItem.rate * (decimal)_data.taxConRate;
                    _data.epfContribution = payItem.rate * (decimal)_data.epfConRate;
                }

                _context.SaveChanges();
            });

            //transaction.Commit();

            using var transaction = BeginTransaction();

            Parallel.ForEach(_emp, emp =>
            {
                decimal _epfTot = _payrollData.Where(o => o.epf == emp.epf).Sum(w => w.epfContribution);
                decimal _taxTot = _payrollData.Where(o => o.epf == emp.epf).Sum(w => w.taxContribution);
            });

            transaction.Commit();
        }

        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();

            return transaction.GetDbTransaction();
        }
    }
}
