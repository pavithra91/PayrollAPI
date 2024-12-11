using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayrollAPI.Models.HRM
{
    public class Supervisor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string userId { get; set; }

        // Fks
        public Employee? epf { get; set; }

        public bool isActive { get; set; }
        public bool isManager { get; set; }
        public bool? isTempSupervisor { get; set; }
        public DateTime? expireDate { get; set; }

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
