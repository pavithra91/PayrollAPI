namespace Leave.Contracts.Response
{
    public class BungalowRateResponse
    {
        public int id { get; set; }
        public string bungalowName { get; init; }
        public string categoryName { get; init; }
        public decimal payDayCost { get; init; }
        public string? createdBy { get; init; }
    }

    public class BungalowRatesResponse
    {
        public IEnumerable<BungalowRateResponse> Items { get; init; } = Enumerable.Empty<BungalowRateResponse>();
    }
}
