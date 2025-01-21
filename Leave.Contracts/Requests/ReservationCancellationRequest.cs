namespace Leave.Contracts.Requests
{
    public class ReservationConfirmationRequest
    {
        public int id { get; init; }
        public string lastUpdateBy { get; init; }
    }
    public class ReservationCancellationRequest
    {
        public int id { get; init; }
        public string lastUpdateBy { get; init; }
    }
}
