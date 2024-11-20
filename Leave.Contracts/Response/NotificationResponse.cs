namespace Leave.Contracts.Response
{
    public class NotificationResponse
    {
        public int id { get; init; }
        public int epf { get; init; }
        public string? description { get; init; }
        public bool readed { get; init; } = false;
        public int type { get; init; }
        public string? target { get; init; }
        public string? status { get; init; }
        public DateTime date { get; init; }
    }
}
