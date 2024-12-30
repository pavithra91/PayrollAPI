using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PayrollAPI.Models.Reservation;
using MySql.EntityFrameworkCore.Extensions;

namespace PayrollAPI.Data.EntityMapping.Reservation
{
    public class CancellationChargesMapping : IEntityTypeConfiguration<CancellationCharges>
    {
        public void Configure(EntityTypeBuilder<CancellationCharges> builder)
        {
            builder
                .HasOne(s => s.reservation)
                .WithOne(x => x.cancellationCharges)
                .HasForeignKey<CancellationCharges>(x => x.reservationID);

            builder
                .Property(s => s.createdDate)
                .ForMySQLHasDefaultValueSql("(CURDATE())");

            builder
                .Property(s => s.createdTime)
                .ForMySQLHasDefaultValueSql("(CURTIME())");
        }
    }
}
