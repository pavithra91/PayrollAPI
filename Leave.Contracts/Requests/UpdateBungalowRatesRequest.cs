using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leave.Contracts.Requests
{
    public class UpdateBungalowRatesRequest
    {
        public int bungalowId { get; init; }
        public Rates[] rates { get; init; }
        public string lastupdateBy { get; init; }
    }

    public class Rates
    {
        public int rateId { get; init; }
        public string categoryName { get; init; }
        public decimal amount { get; init; }
    }
}
