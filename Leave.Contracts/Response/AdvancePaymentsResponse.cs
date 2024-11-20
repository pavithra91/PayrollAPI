using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leave.Contracts.Response
{
    public class AdvancePaymentsResponse
    {
        public IEnumerable<AdvancePaymentResponse> Items { get; init; } = Enumerable.Empty<AdvancePaymentResponse>();
    }
}
