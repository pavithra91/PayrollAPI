namespace Leave.Contracts.Response
{
    public class LeaveBalanceResponse
    {
        public string epf { get; init; }
        public int year { get; init; }
        public string leaveTypeName { get; init; }
        public decimal allocatedLeave { get; init; }
        public decimal usedLeave { get; init; }
        public decimal remainingLeave { get; init; }
        public decimal? carrForwardLeave { get; init; }
    }
}
