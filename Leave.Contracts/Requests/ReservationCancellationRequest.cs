namespace Leave.Contracts.Requests
{
    public class ReservationCancellationRequest
    {
        public int id { get; init; }
        public string lastUpdateBy { get; init; }
    }
}
