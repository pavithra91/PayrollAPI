using Microsoft.EntityFrameworkCore;
using static PayrollAPI.Data.EntityMapping.StatusMapper;

namespace PayrollAPI.Models.Reservation
{
    [Keyless]
    public class Reservation_Payments_View
    {
        public int reservationId { get; set; }
        public string epf { get; set; }
        public DateTime checkInDate { get; set; }
        public DateTime checkOutDate { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public BookingStatus status { get; set; }
        public float amount { get; set; }
        public string chargeType { get; set; }
        
    }
}
