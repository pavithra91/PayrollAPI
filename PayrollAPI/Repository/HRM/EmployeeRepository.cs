using Microsoft.EntityFrameworkCore;
using PayrollAPI.Data;
using PayrollAPI.Interfaces.HRM;
using PayrollAPI.Models.HRM;

namespace PayrollAPI.Repository.HRM
{
    public class EmployeeRepository : IEmployee
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HRMDBConnect _context;
        public EmployeeRepository(HRMDBConnect db, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _context = db;
        }
        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            return await Task.FromResult(_context.Employee
                .Include(x=>x.empGrade)
                .AsEnumerable());
        }

        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            var employeesNotInSupervisors = await _context.Employee
                .Where(e => !_context.Supervisor.Any(s => s.epf.epf == e.epf))
                .Include(e => e.empGrade)
                .ToListAsync();
            return await Task.FromResult(employeesNotInSupervisors);
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            return await Task.FromResult(_context.Employee.Where(x=>x.id==id).FirstOrDefault());
        }

        public async Task<Employee> GetEmployeeByEPF(string epf)
        {
            return await Task.FromResult(_context.Employee.Where(x => x.epf == epf).FirstOrDefault());
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByGrade(string epf)
        {
            Employee emp = _context.Employee.Include(x=>x.empGrade)
                .Where(x => x.epf == epf).FirstOrDefault();

              var list = _context.Employee
                        .Include(x => x.empGrade)
                        .Where(x => x.empGrade.id <= emp.empGrade.id && x.costCenter == emp.costCenter &&
                        x.epf != emp.epf)
                        .AsEnumerable();

            return(list);

            //return await Task.FromResult(_context.Employee
            //    .Include(x=>x.empGrade).OrderByDescending(x=>x.id)
            //    .Where(x => x.empGrade.gradeCode
            //    .StartsWith(emp.empGrade.gradeCode.Substring(0, 1)) && x.costCenter == emp.costCenter)
            //    .AsEnumerable());


            //if (options=="gteq")
            //{            
            //    return await Task.FromResult(_context.Employee
            //            .Include(x => x.empGrade)
            //            .Where(x => x.empGrade.id >= empgrade.id && x.costCenter == costCenter)
            //            .AsEnumerable());
            //}
            //else if(options=="lteq")
            //{
            //    return await Task.FromResult(_context.Employee
            //            .Include(x => x.empGrade)
            //            .Where(x => x.empGrade.id <= empgrade.id && x.costCenter == costCenter)
            //            .AsEnumerable());
            //}
            //else
            //{
            //    return await Task.FromResult(_context.Employee
            //            .Include(x => x.empGrade)
            //            .Where(x => x.empGrade.gradeCode == grade && x.costCenter == costCenter)
            //            .AsEnumerable());
            //}
        }


        public async Task<Supervisor?> GetSupervisor(int id)
        {
            var _supervisor = _context.Supervisor.SingleOrDefault(x => x.id == id);
            return await Task.FromResult(_supervisor);
        }

        public async Task<Supervisor?> GetSupervisorByEPF(string epf)
        {
            var _supervisor = _context.Supervisor
                .Include(x => x.epf).Where(x => x.epf.epf == epf).FirstOrDefault();
            return await Task.FromResult(_supervisor);
        }
        public async Task<bool> CreateSupervisor(Supervisor supervisor)
        {
            _context.Supervisor.Add(supervisor);
            await _context.SaveChangesAsync();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateSupervisor(int id, Supervisor supervisor)
        {
            var existingSupervisor = await _context.Supervisor.FindAsync(id);

            if (existingSupervisor is null)
            {
                return await Task.FromResult(false);
            }
            else
            {
                existingSupervisor.isActive = supervisor.isActive;
                existingSupervisor.isManager = supervisor.isManager;
                existingSupervisor.lastUpdateBy = supervisor.lastUpdateBy;
                existingSupervisor.lastUpdateDate = supervisor.lastUpdateDate;
                existingSupervisor.lastUpdateTime = supervisor.lastUpdateTime;

                await _context.SaveChangesAsync();

                return await Task.FromResult(true);
            }
        }

        public async Task<bool> RequestAdvancePayment(AdvancePayment advancePayment)
        {
            try
            {
                var _previousRequest = _context.AdvancePayment.Where(x => x.employee == advancePayment.employee && x.period == advancePayment.period).FirstOrDefault();
                if (_previousRequest == null)
                {
                    using var scope = _serviceProvider.CreateScope();

                    var dbConnect = scope.ServiceProvider.GetService<DBConnect>();

                    var _advance_CutoffDate = dbConnect.Sys_Properties.Where(x => x.variable_name == "Advance_Cutoff_Date")
                        .FirstOrDefault();

                    DateTime currentDate = DateTime.Now;
                    DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

                    int daysToAdd = Convert.ToInt32(_advance_CutoffDate.variable_value); 
                    DateTime curPeriod = firstDayOfMonth.AddDays(daysToAdd);

                    if(curPeriod < DateTime.Now)
                    {
                        if(currentDate.Month + 1 > 12)
                        {
                            advancePayment.period = Convert.ToInt32(currentDate.Year + 1 + "" + "01");
                        }
                        else
                        {
                            advancePayment.period = Convert.ToInt32(currentDate.Year + "" + (currentDate.Month + 1).ToString("D2"));
                        }        
                    }

                    _context.AdvancePayment.Add(advancePayment);
                    await _context.SaveChangesAsync();

                    return await Task.FromResult(true);
                }
                else
                {
                    return await Task.FromResult(false);
                }
            }
            catch(Exception ex)
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<IEnumerable<AdvancePayment>> GetAdvancePayment(int period)
        {
            return await Task.FromResult(_context.AdvancePayment.Where(x => x.period == period && x.status == ApprovalStatus.Pending)
                .Include(x=>x.employee)
                .AsEnumerable());
        }
    }
}
