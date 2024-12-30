using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PayrollAPI.Models.HRM;
using static PayrollAPI.Data.EntityMapping.StatusMapper;

namespace PayrollAPI.Models.Reservation
{
    public class Bungalow_Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int companyCode { get; set; }
        public Employee employee { get; set; }
        public Bungalow bungalow { get; set; }
        public RaffleDraw raffleDraw { get; set; }
        public ReservationCategory reservationCategory { get; set; }
        public CancellationCharges cancellationCharges { get; set; }

        public DateTime checkInDate { get; set; }
        public DateTime checkOutDate { get; set; }
        public int noOfAdults { get; set; }
        public int noOfChildren { get; set; }
        public int totalPax { get; set; }
        [Column(TypeName = "varchar(15)")]
        public string contactNumber_1 { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string? contactNumber_2 { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string? nicNo { get; set; }

        [Column(TypeName = "varchar(300)")]
        public string? comments { get; set; }

        public BookingStatus bookingStatus { get; set; }

        public BookingPriority bookingPriority { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? reservationCost { get; set; }

        // Logs
        [Column(TypeName = "varchar(10)")]
        public string? createdBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdTime { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? lastUpdateBy { get; set; }
        public DateTime? lastUpdateDate { get; set; }
        public DateTime? lastUpdateTime { get; set; }
    }
}
