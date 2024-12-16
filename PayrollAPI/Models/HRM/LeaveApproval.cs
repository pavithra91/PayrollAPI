using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayrollAPI.Models.HRM
{
    public class LeaveApproval
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string epf { get; set; }

        // Fks
        public LeaveRequest requestId { get; set; }

        public WorkflowTypes level { get; set; }

        public Supervisor approver_id { get; set; }

        public ApprovalStatus status { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? comments { get; set; }
        public bool tempApproval { get; set; }


        // Logs
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdTime { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? lastUpdateBy { get; set; }
        public DateTime? lastUpdateDate { get; set; }
        public TimeSpan? lastUpdateTime { get; set; }
    }
}
