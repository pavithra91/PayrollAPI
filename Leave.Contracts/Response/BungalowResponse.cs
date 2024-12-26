namespace Leave.Contracts.Response
{
    public class BungalowResponse
    {
        public int id { get; set; }
        public int companyCode { get; init; }
        public string bungalowName { get; init; }
        public string? description { get; init; }
        public string? address { get; init; }
        public string? mainImg { get; init; }
        public int noOfRooms { get; init; }
        public int maxBookingPeriod { get; init; }
        public int maxOccupancy { get; init; }
        public bool isCloded { get; init; }
        public DateTime? reopenDate { get; init; }
        public string contactNumber { get; init; }
        public string? createdBy { get; init; }
        public RatesResponse? bungalowRates { get; init; }
    }

    public class BungalowsResponse
    {
        public IEnumerable<BungalowResponse> Items { get; init; } = Enumerable.Empty<BungalowResponse>();
    }

    public class RateResponse
    {
        public int rateId { get; init; }
        public string categoryName { get; init; }
        public decimal amount { get; init; }
    }

    public class RatesResponse
    {
        public IEnumerable<RateResponse> rates { get; init; } = Enumerable.Empty<RateResponse>();
    }
}
