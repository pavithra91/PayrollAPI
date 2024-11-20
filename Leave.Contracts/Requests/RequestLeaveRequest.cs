namespace Leave.Contracts.Requests
{
    public class RequestLeaveRequest
    {
        public string epf { get; init; }
        public string leaveType { get; init; }        
        public string reason { get; init; }
        public DateTime startDate { get; init; }
        public DateTime endDate { get; init; }
        public decimal noOfDays { get; init; }
        public bool isHalfDay { get; init; }   
        public string? halfDayType { get; init; }
        public DateTime? lieuLeaveDate { get; init; }
        public string actDelegate { get; init; }
        public string requestBy {  get; init; }

    }
}
