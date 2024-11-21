namespace Leave.Contracts.Requests
{
    public class CancelLeaveRequest
    {
        public int leaveRequestId { get; init; }
        public string? cancelBy { get; init; }
    }
}
