namespace Leave.Contracts.Response
{
    public class LeaveHistoryResponse
    {
        public IEnumerable<LeaveRequestResponse> Items { get; init; } = Enumerable.Empty<LeaveRequestResponse>();
    }
}
