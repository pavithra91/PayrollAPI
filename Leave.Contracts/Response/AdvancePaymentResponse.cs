namespace Leave.Contracts.Response
{
    public class AdvancePaymentResponse
    {
        public int id { get; init; }
        public string epf { get; init; }
        public int period { get; init; }
        public bool isFullAmount { get; init; }
        public decimal amount { get; init; }
        public string status { get; init; }
        public string? createdBy { get; init; }
    }


    public class AdvancePaymentDisplayResponse
    {
        public int id { get; init; }
        public string epf { get; init; }
        public string empName { get; init; }
        public int period { get; init; }
        public bool isFullAmount { get; init; }
        public decimal amount { get; init; }
        public string status { get; init; }
        public string? createdBy { get; init; }
    }
}
