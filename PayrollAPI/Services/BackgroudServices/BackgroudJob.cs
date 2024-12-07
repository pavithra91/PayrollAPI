using PayrollAPI.Data;
using PayrollAPI.Interfaces.HRM;
using PayrollAPI.Models;
using PayrollAPI.Models.HRM;
using Quartz;

namespace PayrollAPI.Services.BackgroudServices
{
    public class BackgroudJob : IJob
    {
        private readonly IEmployee _employee;
        private readonly ILogger<BackgroudJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        public BackgroudJob(ILogger<BackgroudJob> logger, IEmployee employee, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _employee = employee;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                int period = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                var advancePayments = await _employee.GetAdvancePayment(period);

                using var scope = _serviceProvider.CreateScope();

                var dbConnect = scope.ServiceProvider.GetService<DBConnect>();

                IEnumerable<Sys_Properties> sys_Properties = dbConnect.Sys_Properties
                     .Where(x => x.groupName == "Email_Advance_Payment" || x.groupName == "Email_Config")
                     .AsEnumerable();

                sys_Properties.Where(o => o.variable_name == "Email_Subject")
                    .FirstOrDefault().variable_value = string.Format(sys_Properties
                    .Where(o => o.variable_name == "Email_Subject")
                    .FirstOrDefault().variable_value, period);

                AdvancePaymentPrint print = new AdvancePaymentPrint();
                byte[] data = print.GenerateAdvancePaymentReport(advancePayments, period);


                EmailSender emailSender = new EmailSender();
                await emailSender.SendEmail(sys_Properties, data);

                _logger.LogInformation("Job executed at: " + DateTime.Now);
                //return Task.CompletedTask;
            }
            catch (Exception ex) 
            {
                using var scope = _serviceProvider.CreateScope();

                var dbConnect = scope.ServiceProvider.GetService<DBConnect>();

                IEnumerable<Sys_Properties> sys_Properties = dbConnect.Sys_Properties
                     .Where(x => x.groupName == "Email_Exception" || x.groupName == "Email_Config")
                     .AsEnumerable();

                EmailSender emailSender = new EmailSender();
                await emailSender.SendEmail(sys_Properties);
            }
        }
    }
}
