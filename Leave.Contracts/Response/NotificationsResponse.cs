using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leave.Contracts.Response
{
    public class NotificationsResponse
    {
        public IEnumerable<NotificationResponse> Items { get; init; } = Enumerable.Empty<NotificationResponse>();
    }
}
