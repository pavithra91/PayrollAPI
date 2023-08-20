using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollAPI.Models
{
    public class Temp_Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int epf { get; set; }
        public int period { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string? empName { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string? costCenter { get; set; }

        public int gradeCode { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string? empGrade { get; set; }

        public int paymentType { get; set; }
        public int bankCode { get; set; }
        public int branchCode { get; set; }
        public int accountNo { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? createdBy { get; set; }
        public DateTime createdDate { get; set; }
    }
}
