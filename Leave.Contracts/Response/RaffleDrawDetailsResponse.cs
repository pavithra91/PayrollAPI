namespace Leave.Contracts.Response
{
    public class RaffleDrawDetailsResponse
    {
        public int id { get; init; }
        public int companyCode { get; init; }
        public int rank { get; init; }
        public string epf { get; init; }
        public string empName { get; init; }
        public string bungalowName { get; init; }
        public string contactNumber { get; init; }
        public int noOfPax { get; init; }
        public DateTime checkInDate { get; init; }
        public DateTime checkOutDate { get; init; }
        public decimal? cost { get; init; }
    }

    public class RaffleDrawDetailsResponses
    {
        public IEnumerable<RaffleDrawDetailsResponse> Items { get; init; } = Enumerable.Empty<RaffleDrawDetailsResponse>();
    }
}
