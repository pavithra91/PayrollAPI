namespace Leave.Contracts.Response
{
    public class SupervisorResponse
    {
        public int id { get; init; }
        public string userId { get; init; }
        public string epf { get; init; }
        public string? grade { get; init; }
        public string? empName { get; init; }
        public bool isActive { get; init; }
        public string? createdBy { get; init; }
    }
}
