namespace Leave.Contracts.Response
{
    public class LeaveTypeResponse
    {
        public int leaveTypeId { get; init; }
        public string leaveTypeName { get; init; }
        public string description { get; init; }
        public decimal maxDays { get; init; }
        public bool carryForwardAllowed { get; init; }
        public string? createdBy { get; init; }
    }
}
