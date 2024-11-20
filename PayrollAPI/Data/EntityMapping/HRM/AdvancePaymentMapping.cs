using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PayrollAPI.Models.HRM;
using MySql.EntityFrameworkCore.Extensions;

namespace PayrollAPI.Data.EntityMapping.HRM
{
    public class AdvancePaymentMapping: IEntityTypeConfiguration<AdvancePayment>
    {
        public void Configure(EntityTypeBuilder<AdvancePayment> builder)
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
