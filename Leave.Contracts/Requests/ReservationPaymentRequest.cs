namespace Leave.Contracts.Requests
{
    public class ReservationPaymentRequest
    {
        public int category { get; init; }
        public DateTime fromDate { get; init; }
        public DateTime toDate { get; init; }
    }
}
