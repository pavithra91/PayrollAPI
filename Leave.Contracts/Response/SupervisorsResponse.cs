namespace Leave.Contracts.Response
{
    public class SupervisorsResponse
    {
        public IEnumerable<SupervisorResponse> Items { get; init; } = Enumerable.Empty<SupervisorResponse>();
    }
}
