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
    }
}
