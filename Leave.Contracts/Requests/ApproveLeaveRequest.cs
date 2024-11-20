namespace Leave.Contracts.Requests
{
    public class ApproveLeaveRequest
    {
        public int requestId { get; init; }
        public bool isDelegate { get; init; }
        public string approver { get; init; }
        public string status { get; init; }
        public string? comment { get; init; }
    }
}
