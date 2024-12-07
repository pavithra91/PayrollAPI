using Microsoft.Extensions.Options;
using PayrollAPI.Models.HRM;

namespace PayrollAPI.Interfaces.HRM
{
    public interface IEmployee
    {
        Task<IEnumerable<Employee>> GetAllEmployees();
        Task<Employee> GetEmployeeById(int id);
        Task<Employee> GetEmployeeByEPF(string epf);
        Task<IEnumerable<Employee>> GetEmployeesByGrade(string epf);


        Task<bool> RequestAdvancePayment(AdvancePayment advancePayment);
        Task<IEnumerable<AdvancePayment>> GetAdvancePayment(int period);
    }
}
