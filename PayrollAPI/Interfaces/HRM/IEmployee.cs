using Microsoft.Extensions.Options;
using PayrollAPI.Models.HRM;

namespace PayrollAPI.Interfaces.HRM
{
    public interface IEmployee
    {
        Task<IEnumerable<Employee>> GetAllEmployees();
        Task<IEnumerable<Employee>> GetEmployees();
        Task<Employee> GetEmployeeById(int id);
        Task<Employee> GetEmployeeByEPF(string epf);
        Task<IEnumerable<Employee>> GetEmployeesByGrade(string epf);

        Task<IEnumerable<Supervisor>> GetAllSupervisors(string costCenter);
        Task<Supervisor?> GetSupervisor(int id);
        Task<Supervisor?> GetSupervisorByEPF(string epf);
        Task<bool> CreateSupervisor(Supervisor supervisor);
        Task<bool> UpdateSupervisor(int id, Supervisor supervisor);


        Task<bool> RequestAdvancePayment(AdvancePayment advancePayment);
        Task<IEnumerable<AdvancePayment>> GetMyAdvancePayment(string epf);
        Task<IEnumerable<AdvancePayment>> GetAdvancePayment(int period);
    }
}
