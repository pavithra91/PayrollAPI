using Microsoft.AspNetCore.Http.Features;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;

namespace PayrollAPI.Repository
{
    public class DataRepository : IDatatransfer
    {
        private readonly DBConnect _context;
        public DataRepository(DBConnect db)
        {
            _context = db;
        }

        public MsgDto ConfirmDataTransfer(int period)
        {
            ICollection<Temp_Employee> _employeeList = _context.Temp_Employee.Where(o=>o.period == period).ToList();
            
            IList<Employee_Data> _newEmpList = new List<Employee_Data>();

            foreach(Temp_Employee emp in _employeeList) 
            {
                _newEmpList.Add(new Employee_Data()
                {
                    epf = emp.epf,
                    period = emp.period,
                    empName = emp.empName,
                    costCenter = emp.costCenter,
                    empGrade = emp.empGrade,
                    gradeCode = emp.gradeCode,
                    paymentType = emp.paymentType,
                    bankCode = emp.bankCode,
                    branchCode = emp.branchCode,
                    accountNo = emp.branchCode,
                    status = true
                });
            }
            _context.BulkInsert(_newEmpList);

            ICollection<Temp_Payroll> _payItemList = _context.Temp_Payroll.Where(o => o.period == period).ToList();
            ICollection<PayCode> _payCode = _context.PayCode.ToList();

            IList<Payroll_Data> _newPayrollData = new List<Payroll_Data>();

            foreach (PayCode payCode in _payCode)
            {
                ICollection<Temp_Payroll> _tempList = _payItemList.Where(w => w.payCode == payCode.payCode).ToList();

                foreach (Temp_Payroll payItem in _tempList)
                {
                    _newPayrollData.Add(new Payroll_Data()
                    {
                        epf = payItem.epf,
                        period = payItem.period,
                        othours = payItem.othours,
                        payCategory = payItem.payCategory,
                        payCode = payItem.payCode,
                        calCode = payCode.calCode,
                        paytype = payItem.paytype,
                        costcenter = payItem.costcenter,
                        payCodeType = payItem.payCodeType,
                        amount = payItem.amount,
                        balanceamount = payItem.balanceamount,
                        epfConRate = payItem.epfConRate,
                        epfContribution = (payItem.amount * (decimal) payItem.epfConRate),
                        taxConRate = payItem.taxConRate,
                        taxContribution = (payItem.amount * (decimal)payItem.taxConRate),
                    });
                }
            }

            _context.BulkInsert(_newPayrollData);

            MsgDto _msg = new MsgDto();
            _msg.MsgCode = 'S';
            _msg.Message = "";

            return _msg;
        }
    }
}
