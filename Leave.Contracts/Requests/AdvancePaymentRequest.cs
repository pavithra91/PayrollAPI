using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leave.Contracts.Requests
{
    public class AdvancePaymentRequest
    {
        public string epf { get; init; }
        public bool isFullAmount { get; init; }
        public decimal amount { get; init; }
        public string? createdBy { get; init; }
    }
}
