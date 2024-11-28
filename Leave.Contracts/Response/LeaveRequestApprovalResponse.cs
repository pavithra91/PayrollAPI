namespace Leave.Contracts.Response
{
    public class LeaveRequestApprovalResponse
    {
        public LeaveRequestResponse leaveRequest {  get; init; }
        public IEnumerable<LeaveApprovalResponse> Approvals { get; init; } = Enumerable.Empty<LeaveApprovalResponse>();
    }


    public class LeaveRequestsApprovalResponse
    {
        public IEnumerable<LeaveRequestApprovalResponse> items { get; init; } = Enumerable.Empty<LeaveRequestApprovalResponse>();
    }
}
