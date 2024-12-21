using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PayrollAPI.Models.HRM
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? userID { get; set; }

        public int companyCode { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string? costCenter { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string epf { get; set; }

        [Column(TypeName = "varchar(60)")]
        public string? empName { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string? role { get; set; }

        // Fks
        public EmployeeGrade? empGrade { get; set; }

        public WorkflowTypes? workflowLevel { get; set; }

        [JsonIgnore]
        public ICollection<Supervisor> supervisors { get; set; } = new HashSet<Supervisor>();

        [JsonIgnore]
        public ICollection<EmpApprovals> empApprovals { get; set; } = new HashSet<EmpApprovals>();
        [JsonIgnore]
        public ICollection<AdvancePayment> advancePayments { get; set; } = new HashSet<AdvancePayment>();
        [JsonIgnore]
        public ICollection<Reservation.Bungalow_Reservation> reservations { get; set; } = new HashSet<Reservation.Bungalow_Reservation>();


        [Column(TypeName = "boolean")]
        public bool status { get; set; }

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
