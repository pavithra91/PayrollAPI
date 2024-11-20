using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PayrollAPI.Models.HRM
{
    public class LeaveType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int leaveTypeId { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string leaveTypeName { get; set; }

        [Column(TypeName = "varchar(250)")]
        public string description { get; set; }

        [Column(TypeName = "decimal(4, 1)")]
        public decimal maxDays { get; set; }

        [Column(TypeName = "boolean")]
        public bool carryForwardAllowed { get; set; }


        // FKs
        [JsonIgnore]
        public ICollection <LeaveRequest> leaveRequests { get; set; } = new HashSet<LeaveRequest>();


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
