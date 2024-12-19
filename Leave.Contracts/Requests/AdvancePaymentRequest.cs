namespace Leave.Contracts.Requests
{
    public class AdvancePaymentRequest
    {
        public string epf { get; init; }
        public bool isFullAmount { get; init; }
        public decimal amount { get; init; }
        public string? createdBy { get; init; }
    }

    public class AdvancePaymentProcessingRequest
    {
        public int period { get; init; }
        public string? RequestBy { get; init; }
    }
}
