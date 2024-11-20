using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollAPI.Models.HRM
{
    public class AdvancePayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string epf { get; set; }
        public int period { get; set; }
        public bool isFullAmount { get; set; } = true;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal amount { get; set; }

        public ApprovalStatus status { get; set; }

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
