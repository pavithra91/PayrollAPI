namespace Leave.Contracts.Requests
{
    public class LeaveTypeRequest
    {
        public string leaveTypeName { get; init; }
        public string description { get; init; }
        public decimal maxDays { get; init; }
        public bool carryForwardAllowed { get; init; }
        public string? createdBy { get; init; }
    }

    public class UpdateLeaveTypeRequest
    {
        public int leaveTypeId { get; init; }
        public string leaveTypeName { get; init; }
        public string description { get; init; }
        public decimal maxDays { get; init; }
        public bool carryForwardAllowed { get; init; }
        public string? createdBy { get; init; }
    }
}
