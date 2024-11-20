using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leave.Contracts.Response
{
    public class LeaveRequestResponse
    {
        public int requestId { get; init; }
        public string epf { get; init; }
        public string leaveTypeName { get; init; }
        public DateTime startDate { get; init; }
        public DateTime endDate { get; init; }
        public DateTime? lieuLeaveDate { get; init; }
        public string reason { get; init; }
        public bool isHalfDay { get; init; }
        public string? halfDayType { get; init; }
        public string? actingDelegate { get; init; }
        public string? actingDelegateApprovalStatus { get; init; }
        public int currentLevel { get; set; }
        public string? requestStatus { get; set; }
    }
}
