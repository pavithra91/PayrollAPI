using PayrollAPI.Data;
using PayrollAPI.Interfaces.HRM;
using PayrollAPI.Models.HRM;
using PayrollAPI.Models.Payroll;
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
                int period = 0;
                using var scope = _serviceProvider.CreateScope();

                var dbConnect = scope.ServiceProvider.GetService<DBConnect>();

                var _advance_CutoffDate = dbConnect.Sys_Properties.Where(x => x.variable_name == "Advance_Cutoff_Date")
                        .FirstOrDefault();

                DateTime currentDate = DateTime.Now;
                DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

                int daysToAdd = Convert.ToInt32(_advance_CutoffDate.variable_value);
                DateTime curPeriod = firstDayOfMonth.AddDays(daysToAdd);

                if (curPeriod < DateTime.Now)
                {
                    if (currentDate.Month + 1 > 12)
                    {
                        period = Convert.ToInt32(currentDate.Year + 1 + "" + "01");
                    }
                    else
                    {
                        period = Convert.ToInt32(currentDate.Year + "" + (currentDate.Month + 1).ToString("D2"));
                    }
                }

                var advancePayments = await _employee.GetAdvancePayment(period);

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
                await emailSender.SendEmail(sys_Properties, data, "AdvancePayment.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

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
