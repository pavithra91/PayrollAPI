 using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using PayrollAPI.Models;

namespace PayrollAPI.Data
{
    public class DBConnect: DbContext
    {
        public DBConnect(DbContextOptions<DBConnect> options) : base(options)
        {

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
        public DbSet<User> User { get; set; }

        public DbSet<LoginInfo> LoginInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temp_Employee>()
                    .Property(s => s.createdDate)
                    .ForMySQLHasDefaultValueSql("CURDATE()");
        }
    }
}
