using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leave.Contracts.Requests
{
    public class BungalowRequest
    {
        public int companyCode { get; init; }
        public string bungalowName { get; init; }
        public string? description { get; init; }
        public string? address { get; init; }
        public string? mainImg { get; init; }
        public decimal perDayCost { get; init; }
        public int noOfRooms { get; init; }
        public int maxBookingPeriod { get; init; }
        public int maxOccupancy { get; init; }
        public bool isClosed { get; init; }
        public DateTime? reopenDate { get; init; }
        public string contactNumber { get; init; }
        public string? createdBy { get; init; }
    }

    public class UpdateBungalowRequest
    {
        public string bungalowName { get; init; }
        public string? description { get; init; }
        public string? address { get; init; }
        public string? mainImg { get; init; }
        public decimal perDayCost { get; init; }
        public int noOfRooms { get; init; }
        public int maxBookingPeriod { get; init; }
        public bool isClosed { get; init; }
        public DateTime? reopenDate { get; init; }
        public string contactNumber { get; init; }
        public string? lastUpdateBy { get; init; }
    }
}
