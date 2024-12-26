namespace Leave.Contracts.Response
{
    public class ReservationResponse
    {
        public int id { get; init; }
        public int companyCode { get; init; }
        public int? empId { get; init; }
        public string epf { get; init; }
        public int? bungalowId { get; init; }
        public string bungalowName { get; init; }
        public string reservationCategory { get; init; }
        public DateTime checkInDate { get; init; }
        public DateTime checkOutDate { get; init; }
        public int noOfAdults { get; init; }
        public int noOfChildren { get; init; }
        public int totalPax { get; init; }
        public string contactNumber_1 { get; init; }
        public string contactNumber_2 { get; init; }
        public string? nicNo { get; init; }
        public string? comments { get; init; }
        public decimal? reservationCost { get; init; }
        public string bookingStatus { get; init; }
        public string createdBy { get; init; }
    }

    public class ReservationsResponse
    {
        public IEnumerable<ReservationResponse> Items { get; init; } = Enumerable.Empty<ReservationResponse>();
    }
}
