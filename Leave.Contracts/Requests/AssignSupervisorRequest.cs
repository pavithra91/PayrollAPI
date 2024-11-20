using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leave.Contracts.Requests
{
    public class AssignSupervisorRequest
    {
        public int id { get; init; }
        public string epf { get; init; }
        public int approvalLevel { get ; init; }
        public List<int> approverNames {get; init; }

        public string updateBy {  get; init; }
    }
}
