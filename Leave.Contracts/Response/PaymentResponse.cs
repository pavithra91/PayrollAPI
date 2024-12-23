namespace Leave.Contracts.Response
{
    public class PaymentResponse
    {
        public int id { get; init; }
        public string epf { get; init; }
        public string empName { get; init; }
        public string bankCode { get; init; }
        public string accountNo { get; init; }
        public decimal amount { get; init; }
        public DateTime bankTransferDate { get; init; }
        public string paymentCategory { get; init; }
        public string voucherNo { get; init; }
        public string status { get; init; }
        public string createdBy { get; init; }
    }

    public class PaymentsResponse
    {
        public IEnumerable<PaymentResponse> Items { get; init; } = Enumerable.Empty<PaymentResponse>();
    }
}
