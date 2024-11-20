using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PayrollAPI.Models.HRM;
using MySql.EntityFrameworkCore.Extensions;

namespace PayrollAPI.Data.EntityMapping.HRM
{
    public class LeaveApprovalMappings : IEntityTypeConfiguration<LeaveApproval>
    {
        public void Configure(EntityTypeBuilder<LeaveApproval> builder)
        {
            builder
                .Property(s => s.createdDate)
                .ForMySQLHasDefaultValueSql("(CURDATE())");

            builder
                .Property(s => s.createdTime)
                .ForMySQLHasDefaultValueSql("(CURTIME())");

            builder
                .Property(la => la.status)
                .HasConversion<string>();
        }
    }
}
