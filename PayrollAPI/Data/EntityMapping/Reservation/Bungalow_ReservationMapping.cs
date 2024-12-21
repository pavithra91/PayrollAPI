using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySql.EntityFrameworkCore.Extensions;
using PayrollAPI.Models.Reservation;

namespace PayrollAPI.Data.EntityMapping.Reservation
{
    public class Bungalow_ReservationMapping : IEntityTypeConfiguration<Bungalow_Reservation>
    {
        public void Configure(EntityTypeBuilder<Bungalow_Reservation> builder)
        {
            builder
                .HasOne(s => s.raffleDraw)
                .WithOne(x => x.bungalow_Reservation)
                .HasForeignKey<RaffleDraw>(x => x.reservationID);

            builder
                .Property(s => s.createdDate)
                .ForMySQLHasDefaultValueSql("(CURDATE())");

            builder
                .Property(s => s.createdTime)
                .ForMySQLHasDefaultValueSql("(CURTIME())");
        }
    }
}
