using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySql.EntityFrameworkCore.Extensions;
using PayrollAPI.Models.HRM;

namespace PayrollAPI.Data.EntityMapping.HRM
{
    public class LeaveBalanceMapping: IEntityTypeConfiguration<LeaveBalance>
    {
        public void Configure(EntityTypeBuilder<LeaveBalance> builder)
        {
            builder
                .Property(s => s.createdDate)
                .ForMySQLHasDefaultValueSql("(CURDATE())");

            builder
                .Property(s => s.createdTime)
                .ForMySQLHasDefaultValueSql("(CURTIME())");
        }
    }
}
