using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PayrollAPI.Models.Services;
using MySql.EntityFrameworkCore.Extensions;

namespace PayrollAPI.Data.EntityMapping.Services
{
    public class JobScheduleMapping: IEntityTypeConfiguration<JobSchedule>
    {
        public void Configure(EntityTypeBuilder<JobSchedule> builder)
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
