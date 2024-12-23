namespace Leave.Contracts.Requests
{
    public class PaymentRequest
    {
        public string voucherNo { get; init; }
        public DateTime bankDate { get; init; }
        public string processBy { get; init; }
    }
}
