using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leave.Contracts.Response
{
    public class ReservationPaymentResponse
    {
        public int reservationId { get; set; }
        public string epf { get; set; }
        public DateTime checkInDate { get; set; }
        public DateTime checkOutDate { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public string status { get; set; }
        public float amount { get; set; }
        public string chargeType { get; set; }
    }

    public class ReservationPaymentsResponse
    {
        public IEnumerable<ReservationPaymentResponse> Items { get; init; } = Enumerable.Empty<ReservationPaymentResponse>();
    }
}
