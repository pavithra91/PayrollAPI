using Microsoft.EntityFrameworkCore;
using PayrollAPI.Data;
using PayrollAPI.Interfaces.HRM;
using PayrollAPI.Models.HRM;

namespace PayrollAPI.Repository.HRM
{
    public class EmployeeRepository : IEmployee
    {
        private readonly HRMDBConnect _context;
        public EmployeeRepository(HRMDBConnect db)
        {
            _context = db;
        }
        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            return await Task.FromResult(_context.Employee.AsEnumerable());
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            return await Task.FromResult(_context.Employee.Where(x=>x.id==id).FirstOrDefault());
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByGrade(string grade, string costCenter, string? options)
        {
            EmployeeGrade empgrade = _context.EmployeeGrade.Where(x => x.gradeCode == grade).FirstOrDefault();

            if (options=="gteq")
            {            
                return await Task.FromResult(_context.Employee
                        .Include(x => x.empGrade)
                        .Where(x => x.empGrade.id >= empgrade.id && x.costCenter == costCenter)
                        .AsEnumerable());
            }
            else if(options=="lteq")
            {
                return await Task.FromResult(_context.Employee
                        .Include(x => x.empGrade)
                        .Where(x => x.empGrade.id <= empgrade.id && x.costCenter == costCenter)
                        .AsEnumerable());
            }
            else
            {
                return await Task.FromResult(_context.Employee
                        .Include(x => x.empGrade)
                        .Where(x => x.empGrade.gradeCode == grade && x.costCenter == costCenter)
                        .AsEnumerable());
            }
        }
    }
}
