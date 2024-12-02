using PayrollAPI.Models.HRM;

namespace PayrollAPI.Interfaces.HRM
{
    public interface IEmployee
    {
        Task<IEnumerable<Employee>> GetAllEmployees();
    }
}
