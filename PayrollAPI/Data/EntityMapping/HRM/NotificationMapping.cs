using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySql.EntityFrameworkCore.Extensions;
using PayrollAPI.Models.HRM;

namespace PayrollAPI.Data.EntityMapping.HRM
{
    public class NotificationMapping : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder
                .Property(s => s.createdDate)
                .ForMySQLHasDefaultValueSql("(CURDATE())");
        }
    }
}