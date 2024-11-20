using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leave.Contracts.Response
{
    public class ApprovalWorkflowResponse
    {
        public int? total { get; set; }
        public IEnumerable<ApprovalWorkflowDto> data { get; set; }
    }

    public class ApprovalWorkflowDto
    {
        public int id { get; init; }
        public int epf { get; set; }

        public int? level { get; set; }
        public string approvalLevel { get; set; }
        public List<SupervisorDto> SupervisorList { get; set; }
    }

    public class SupervisorDto
    {
        public string Level { get; set; }
        public int Epf { get; set; }
    }
}
