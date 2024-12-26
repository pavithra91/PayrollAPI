using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static PayrollAPI.Data.EntityMapping.StatusMapper;

namespace PayrollAPI.Models.Reservation
{
    public class RaffleDraw
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int RaffleDrawId { get; set; }

        // Fks
        public int reservationID { get; set; }
        public Bungalow_Reservation bungalow_Reservation { get; set; }
        public int rank { get; set; }

        public BookingStatus bookingStatus { get; set; }

        // Logs
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdTime { get; set; }
    }
}
