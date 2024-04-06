 using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using PayrollAPI.Models;

namespace PayrollAPI.Data
{
    public class DBConnect: DbContext
    {
        public DBConnect(DbContextOptions<DBConnect> options) : base(options)
        {
            this.Database.SetCommandTimeout(180);
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
}
