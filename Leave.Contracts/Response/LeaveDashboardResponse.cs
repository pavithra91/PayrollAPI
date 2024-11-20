using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leave.Contracts.Response
{
    public class LeaveDashboardResponse
    {
        public IEnumerable<LeaveRequestResponse> LeaveHistory { get; init; } = Enumerable.Empty<LeaveRequestResponse>();
        public IEnumerable<LeaveBalanceResponse> LeaveBalance { get; init; } = Enumerable.Empty<LeaveBalanceResponse>();
    }
}
