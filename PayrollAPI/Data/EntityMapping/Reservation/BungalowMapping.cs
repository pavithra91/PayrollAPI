using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySql.EntityFrameworkCore.Extensions;
using PayrollAPI.Models.Reservation;

namespace PayrollAPI.Data.EntityMapping.Reservation
{
    public class BungalowMapping : IEntityTypeConfiguration<Bungalow>
    {
        public void Configure(EntityTypeBuilder<Bungalow> builder)
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
