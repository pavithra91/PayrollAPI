using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySql.EntityFrameworkCore.Extensions;
using PayrollAPI.Models.HRM;
using System.Reflection.Emit;

namespace PayrollAPI.Data.EntityMapping.HRM
{
    public class LeaveRequestMapping: IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder
                .Property(s => s.createdDate)
                .ForMySQLHasDefaultValueSql("(CURDATE())");

            builder
                .Property(s => s.createdTime)
                .ForMySQLHasDefaultValueSql("(CURTIME())");

            builder
                .Property(la => la.actingDelegateApprovalStatus)
                .HasConversion<string>(); 
            
            builder
                .Property(la => la.requestStatus)
                .HasConversion<string>();

            builder
                .Property(la => la.finalStatus)
                .HasConversion<string>();
        }
    }
}
