namespace Leave.Contracts.Response
{
    public class LeaveTypesResponse
    {
        public IEnumerable<LeaveTypeResponse> Items { get; init; } = Enumerable.Empty<LeaveTypeResponse>();
    }
}
