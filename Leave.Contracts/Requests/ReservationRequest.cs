namespace Leave.Contracts.Requests
{
    public class ReservationRequest
    {
        public int companyCode { get; init; }
        public string epf { get; init; }
        public int bungalowid { get; init; }
        public int category { get; init; }
        public DateTime checkInDate { get; init; }
        public DateTime checkOutDate { get; init; }
        public int noOfAdults { get; init; }
        public int noOfChildren { get; init; }
        public int totalPax { get; init; }
        public string contactNumber_1 { get; init; }
        public string contactNumber_2 { get; init; }
        public string? nicNo { get; init; }
        public string? comments { get; init; }        
        public string createdBy { get; init; }
    }
    public class UpdateReservationRequest
    {
        public int companyCode { get; init; }
        public string epf { get; init; }
        public int bungalowid { get; init; }
        public DateTime checkInDate { get; init; }
        public DateTime checkOutDate { get; init; }
        public int noOfAdults { get; init; }
        public int noOfChildren { get; init; }
        public int totalPax { get; init; }
        public string contactNumber_1 { get; init; }
        public string contactNumber_2 { get; init; }
        public string? nicNo { get; init; }
        public string? comments { get; init; }
        public string? bookingStatus { get; init; }
        public string lastUpdateBy { get; init; }
    }
}
