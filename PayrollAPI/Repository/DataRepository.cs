using Microsoft.AspNetCore.Http.Features;
using PayrollAPI.Data;
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

        public bool ConfirmDataTransfer()
        {
            ICollection<Temp_Employee> _employeeList = _context.Temp_Employee.OrderBy(o=>o.epf).ToList();
            
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

            return true;
        }
    }
}
