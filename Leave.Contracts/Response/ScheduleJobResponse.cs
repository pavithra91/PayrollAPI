namespace Leave.Contracts.Response
{
    public class ScheduleJobResponse
    {
        public int id { get; init; }
        public string jobName { get; init; }
        public string groupName { get; init; }
        public string cronExpression { get; init; }
        public bool isActive { get; init; }
        public string? createdBy { get; init; }
    }

    public class ScheduleJobsResponse
    {
        public IEnumerable<ScheduleJobResponse> Items { get; init; } = Enumerable.Empty<ScheduleJobResponse>();
    }
}
