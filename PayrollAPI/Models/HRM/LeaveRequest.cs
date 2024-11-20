using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayrollAPI.Models.HRM
{
    public class LeaveRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int leaveRequestId { get; set; }

        public int epf { get; set; }

        // Fks
        public LeaveType leaveType { get; set; }

        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public DateTime lieuLeaveDate { get; set; }

        [Column(TypeName = "varchar(250)")]
        public string? reason { get; set; }

        [Column(TypeName = "boolean")]
        public bool isHalfDay { get; set; }

        public HalfDayType? halfDayType { get; set; }

        [Column(TypeName = "decimal(4, 1)")]
        public decimal? noOfDays { get; set; }

        public string? actingDelegate { get; set; }

        public ApprovalStatus? actingDelegateApprovalStatus { get; set; }

        public DateTime? actingDelegateApprovedDate { get; set; }
        public DateTime? actingDelegateApprovedTime { get; set; }

        public ApprovalStatus requestStatus { get; set; }
        public int currentLevel { get; set; }

        public FinalStatus? finalStatus { get; set; }

        // Logs
        [Column(TypeName = "varchar(10)")]
        public string? actionedBy { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdTime { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? lastUpdateBy { get; set; }
        public DateTime? lastUpdateDate { get; set; }
        public DateTime? lastUpdateTime { get; set; }
    }

    public enum ApprovalStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public enum FinalStatus
    {
        Approved = 0,
        Rejected = 1,
        Cancelled = 2
    }

    public enum HalfDayType
    {
        Morning = 1,
        Evening = 2,
    }
}
