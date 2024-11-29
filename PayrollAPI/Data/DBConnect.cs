using EntityFramework.Exceptions.MySQL;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using PayrollAPI.Data.EntityMapping.HRM;
using PayrollAPI.Models;
using PayrollAPI.Models.HRM;

namespace PayrollAPI.Data
{
    public class DBConnect : DbContext
    {
        public DBConnect(DbContextOptions<DBConnect> options) : base(options)
        {
            this.Database.SetCommandTimeout(320);
        }

        public DbSet<Calculation> Calculation { get; set; }
        public DbSet<Employee_Data> Employee_Data { get; set; }
        public DbSet<EmpSpecialRate> EmpSpecialRate { get; set; }
        public DbSet<EPF_ETF> EPF_ETF { get; set; }
        public DbSet<PayCode> PayCode { get; set; }
        public DbSet<Payroll_Data> Payroll_Data { get; set; }
        public DbSet<Payrun> Payrun { get; set; }
        public DbSet<Special_Tax_Emp> Special_Tax_Emp { get; set; }
        public DbSet<Tax_Calculation> Tax_Calculation { get; set; }
        public DbSet<Temp_Employee> Temp_Employee { get; set; }
        public DbSet<Temp_Payroll> Temp_Payroll { get; set; }
        public DbSet<TotPayCode> TotPayCode { get; set; }
        public DbSet<SAPTotPayCode> SAPTotPayCode { get; set; }
        public DbSet<Unrecovered> Unrecovered { get; set; }
        public DbSet<User> User { get; set; }

        public DbSet<Category> Category { get; set; }
        public DbSet<Article> Article { get; set; }

        public DbSet<LoginInfo> LoginInfo { get; set; }

        public DbSet<SysLog> SysLog { get; set; }

        public DbSet<Sys_Properties> Sys_Properties { get; set; }
        public DbSet<PaySheet_Log> PaySheet_Log { get; set; }
        public DbSet<BackgroudJobs> BackgroudJobs { get; set; }

        public DbSet<OTHours_View> GetOTDetails { get; set; }
        public DbSet<Payroll_Summary_View> GetSummaryDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder modelBuilder)
        {
            modelBuilder.UseExceptionProcessor();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<OTHours_View>().Entity<OTHours_View>().HasNoKey();
            modelBuilder.Ignore<Payroll_Summary_View>().Entity<Payroll_Summary_View>().HasNoKey();

            modelBuilder.Entity<Calculation>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<Calculation>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<EmpSpecialRate>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<EmpSpecialRate>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<EPF_ETF>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<EPF_ETF>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<PayCode>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<PayCode>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<Special_Tax_Emp>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<Special_Tax_Emp>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<Tax_Calculation>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<Tax_Calculation>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<Temp_Employee>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<Temp_Employee>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<Temp_Payroll>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<Temp_Payroll>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<TotPayCode>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<TotPayCode>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<SAPTotPayCode>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<SAPTotPayCode>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<Unrecovered>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<Unrecovered>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<User>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<User>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<Article>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<Article>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<Sys_Properties>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<Sys_Properties>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<BackgroudJobs>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("(CURDATE())");

            modelBuilder.Entity<BackgroudJobs>()
                    .Property(s => s.createdTime)
                    .ForMySQLHasDefaultValueSql("(CURTIME())");

            modelBuilder.Entity<Employee_Data>()
                    .Property(s => s.status)
                    .ForMySQLHasDefaultValueSql("1");

            modelBuilder.Entity<Employee_Data>()
                    .Property(s => s.isPaysheetGenerated)
                    .ForMySQLHasDefaultValueSql("1");

            modelBuilder.Entity<Payroll_Data>()
                    .Property(s => s.displayOnPaySheet)
                    .ForMySQLHasDefaultValueSql("1");
        }
    }

    public class HRMDBConnect : DbContext
    {
        public HRMDBConnect(DbContextOptions<HRMDBConnect> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EmpApprovalsMapping());
            modelBuilder.ApplyConfiguration(new LeaveApprovalMappings());
            modelBuilder.ApplyConfiguration(new LeaveBalanceMapping());
            modelBuilder.ApplyConfiguration(new LeaveRequestMapping());
            modelBuilder.ApplyConfiguration(new LeaveTypeMapping());
            modelBuilder.ApplyConfiguration(new SupervisorMapping());
            modelBuilder.ApplyConfiguration(new WorkflowTypesMapping()); 
            modelBuilder.ApplyConfiguration(new NotificationMapping()); 
            modelBuilder.ApplyConfiguration(new AdvancePaymentMapping());
            modelBuilder.ApplyConfiguration(new EmployeeMapping());
        }

        public DbSet<EmpApprovals> EmpApprovals => Set<EmpApprovals>();
        public DbSet<EmpApprovalWorkflow> EmpApprovalWorkflow => Set<EmpApprovalWorkflow>();
        public DbSet<LeaveApproval> LeaveApproval => Set<LeaveApproval>();
        public DbSet<LeaveBalance> LeaveBalance => Set<LeaveBalance>();
        public DbSet<LeaveRequest> LeaveRequest => Set<LeaveRequest>();
        public DbSet<LeaveType> LeaveType => Set<LeaveType>();
        public DbSet<Supervisor> Supervisor => Set<Supervisor>();
        public DbSet<WorkflowTypes> WorkflowTypes => Set<WorkflowTypes>();
        public DbSet<Notification> Notification => Set<Notification>();
        public DbSet<AdvancePayment> AdvancePayment => Set<AdvancePayment>();
        public DbSet<Employee> Employee => Set<Employee>();
    }
}
