
namespace Leave.Contracts.Requests
{
    public class ScheduleJobRequest
    {
        public string jobName { get; init; }
        public string groupName { get; init; }
        public string cronExpression { get; init; }
        public string? createdBy { get; init; }
    }

    public class UpdateScheduleJobRequest
    {
        public int id { get; init; }
        public string jobName { get; init; }
        public string groupName { get; init; }
        public string cronExpression { get; init; }
        public string? createdBy { get; init; }
    }
}
