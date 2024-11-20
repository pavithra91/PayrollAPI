using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayrollAPI.Models.HRM
{
    public class LeaveBalance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int epf { get; set; }

        public int year { get; set; }

        public LeaveType leaveType { get; set; }

        [Column(TypeName = "decimal(4, 1)")]
        public decimal allocatedLeaves { get; set; }

        [Column(TypeName = "decimal(4, 1)")]
        public decimal usedLeaves { get; set; } = 0m;

        [Column(TypeName = "decimal(4, 1)")]
        public decimal remainingLeaves { get; set; }

        [Column(TypeName = "decimal(4, 1)")]
        public decimal carryForwardLeaves { get; set; } = 0m;


        // Logs
        [Column(TypeName = "varchar(10)")]
        public string? createdBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdTime { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? lastUpdateBy { get; set; }
        public DateTime? lastUpdateDate { get; set; }
        public DateTime? lastUpdateTime { get; set; }
    }
}
