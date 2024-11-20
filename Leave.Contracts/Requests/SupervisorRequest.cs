namespace Leave.Contracts.Requests
{
    public class SupervisorRequest
    {
        public string userId { get; init; }
        public string epf { get; init; }
        public bool isActive { get; init; }
        public string? createdBy { get; init; }
    }

    public class UpdateSupervisorRequest
    {
        public int id { get; init; }
        public bool isActive { get; init; }
        public string? createdBy { get; init; }
    }
}
