using Leave.Contracts.Requests;
using LinqToDB;
using Microsoft.EntityFrameworkCore;
using PayrollAPI.Data;
using PayrollAPI.Interfaces.HRM;
using PayrollAPI.Models;
using PayrollAPI.Models.HRM;
using PayrollAPI.Services;

namespace PayrollAPI.Repository.HRM
{
    public class EmployeeRepository : IEmployee
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HRMDBConnect _context;
        Common com = new Common();
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
            var employeesNotInSupervisors = _context.Employee
                .Where(e => !_context.Supervisor.Any(s => s.epf.epf == e.epf))
                .Include(e => e.empGrade).ToList();
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
            try
            {
                Employee emp = _context.Employee.Include(x => x.empGrade)
                    .Where(x => x.epf == epf).FirstOrDefault();

                var eligibleGrades = GradeHelper.GetEligibleGrades(emp.empGrade.gradeCode);

                List<Employee> result = null;

                // Start by querying with the initial eligible grades
                result = QueryEmployeesByGrades(eligibleGrades, emp);

                // If no employees were found, try expanding the eligible grades and query again
                if (result == null || !result.Any())
                {
                    var expandedGrades = new List<string>(eligibleGrades);
                    ExpandEligibleGrades(ref expandedGrades, emp);

                    // Query again with the expanded grades
                    result = QueryEmployeesByGrades(expandedGrades, emp);
                }

                return result;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Supervisor>> GetAllSupervisors(string costCenter)
        {
            return await Task.FromResult(_context.Supervisor
                .Include(x => x.epf)
                .Include(x => x.epf.empGrade)
                .Where(x => x.epf.costCenter == costCenter && x.isActive == true)
                .AsEnumerable());
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

                    DateTime currentDate = com.GetTimeZone();
                    DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

                    int daysToAdd = Convert.ToInt32(_advance_CutoffDate.variable_value); 
                    DateTime curPeriod = firstDayOfMonth.AddDays(daysToAdd);

                    if(curPeriod < com.GetTimeZone())
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

        public async Task<IEnumerable<AdvancePayment>> GetMyAdvancePayment(string epf)
        {
            return await Task.FromResult(_context.AdvancePayment.Where(x => x.employee.epf == epf)
                .Include(x => x.employee)
                .OrderByDescending(x=>x.createdDate)
                .AsEnumerable());
        }

        public async Task<IEnumerable<AdvancePayment>> GetAdvancePayment(int period)
        {
            return await Task.FromResult(_context.AdvancePayment.Where(x => x.period == period && x.status == ApprovalStatus.Pending)
                .Include(x=>x.employee)
                .AsEnumerable());
        }

        public async Task<bool> ProcessAdvancePayment(AdvancePaymentProcessingRequest processingRequest)
        {
            try
            {
                bool hasDataToUpdate = _context.AdvancePayment
                                       .Any(x => x.period == processingRequest.period && x.status == ApprovalStatus.Pending);

                if (!hasDataToUpdate) {
                    return await Task.FromResult(false);
                }

                _context.AdvancePayment.Where(x => x.period == processingRequest.period && x.status == ApprovalStatus.Pending)
                    .UpdateFromQuery(x => new AdvancePayment { status = ApprovalStatus.Approved, lastUpdateBy = processingRequest.RequestBy, lastUpdateDate = com.GetTimeZone(), lastUpdateTime = com.GetTimeZone() });
                await _context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            catch(Exception ex)
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> DeleteAdvancePayment(int id)
        {
            var existingRequest = await _context.AdvancePayment.FindAsync(id);

            if (existingRequest is null)
            {
                return await Task.FromResult(false);
            }
            else
            {
                existingRequest.status = ApprovalStatus.Cancelled;

                await _context.SaveChangesAsync();

                return await Task.FromResult(true);
            }
        }


        private List<Employee> QueryEmployeesByGrades(List<string> eligibleGrades, Employee emp)
        {
            return _context.Employee
                .Include(x => x.empGrade)
                .Where(x => eligibleGrades.Contains(x.empGrade.gradeCode))  // Filter by eligible grades
                .Where(x => x.costCenter == emp.costCenter && x.epf != emp.epf) // Same cost center and exclude current employee
                                                                                //.OrderBy(x => GradeHelper.GetGradeValue(x.empGrade.gradeCode)) // Order by grade (ascending)
                .ToList(); // Fetch the data into memory
        }

        // Helper method to expand eligible grades when no employees are found
        private void ExpandEligibleGrades(ref List<string> eligibleGrades, Employee emp)
        {
            // Check if there are any employees left. If not, keep expanding to include lower grades
            var lastGradeValue = GradeHelper.GetGradeValue(eligibleGrades.Last());

            // Continue expanding until we find employees
            while (eligibleGrades.Any() && !HasEmployeesForGrades(eligibleGrades, emp))
            {
                var nextLowerGrade = GetNextLowerGrade(eligibleGrades.Last());
                if (nextLowerGrade != null)
                {
                    eligibleGrades.Add(nextLowerGrade);
                }
                else
                {
                    break; // Exit if there are no more grades to add
                }
            }
        }

        // Helper method to check if there are any employees for a given set of grades
        private bool HasEmployeesForGrades(List<string> eligibleGrades, Employee emp)
        {
            var employees = _context.Employee
                .Where(x => x.epf != emp.epf)
                .Where(x => eligibleGrades.Contains(x.empGrade.gradeCode))
                .Any(); // Check if any employee exists for these grades
            return employees;
        }

        // Helper method to get the next lower grade for the given grade
        private string GetNextLowerGrade(string currentGrade)
        {
            // Define the grade order
            var gradeOrder = new List<string>
        {
            "A1", "A2", "A3", "A4", "A5", "A6", "A7", "B1", "B2", "B3", "B4", "C1", "C2", "C3", "C4"
        };

            // Find the index of the current grade
            int currentIndex = gradeOrder.IndexOf(currentGrade);
            if (currentIndex < gradeOrder.Count - 1)
            {
                return gradeOrder[currentIndex + 1]; // Return the next lower grade
            }

            return null; // If there are no lower grades, return null
        }
    }
}
