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
        public decimal perDayCost { get; init; }
        public int noOfRooms { get; init; }
        public int maxBookingPeriod { get; init; }
        public int maxOccupancy { get; init; }
        public bool isCloded { get; init; }
        public DateTime? reopenDate { get; init; }
        public string contactNumber { get; init; }
        public string? createdBy { get; init; }
    }

    public class BungalowsResponse
    {
        public IEnumerable<BungalowResponse> Items { get; init; } = Enumerable.Empty<BungalowResponse>();
    }
}
