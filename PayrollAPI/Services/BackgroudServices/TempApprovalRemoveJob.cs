using PayrollAPI.Data;
using PayrollAPI.Interfaces.HRM;
using PayrollAPI.Models.Payroll;
using Quartz;

namespace PayrollAPI.Services.BackgroudServices
{
    public class TempApprovalRemoveJob : IJob
    {
        private readonly IEmployee _employee;
        private readonly ILeave _leave;
        private readonly ILogger<TempApprovalRemoveJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        public TempApprovalRemoveJob(ILogger<TempApprovalRemoveJob> logger, IEmployee employee, ILeave leave, IServiceProvider serviceProvider)
        {
            _employee = employee;
            _logger = logger;
            _leave = leave;
            _serviceProvider = serviceProvider;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var _leaveApproval = _leave.ClearTempApprovals();
                _logger.LogInformation("TempApprovalRemoveJob - Job executed at: " + DateTime.Now);
            }
            catch (Exception ex) 
            {
                using var scope = _serviceProvider.CreateScope();

                var dbConnect = scope.ServiceProvider.GetService<DBConnect>();

                IEnumerable<Sys_Properties> sys_Properties = dbConnect.Sys_Properties
                     .Where(x => x.groupName == "Email_Exception" || x.groupName == "Email_Config")
                     .AsEnumerable();

                EmailSender emailSender = new EmailSender();
                await emailSender.SendEmail(sys_Properties, ex.Message);
            }
        }
    }
}
