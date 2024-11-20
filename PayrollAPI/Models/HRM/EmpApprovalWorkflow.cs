using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PayrollAPI.Models.HRM
{
    public class EmpApprovalWorkflow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public WorkflowTypes? approvalLevels { get; set; }
        public Supervisor? approverId { get; set; }

        public EmpApprovals empApprovalId { get; set; }
    }
}
