using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PayrollAPI.Models
{
    public class Employee_Data
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int companyCode { get; set; }
        public int location { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string epf { get; set; }
        public int period { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string? empName { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string? costCenter { get; set; }

        public int gradeCode { get; set; }

        [Column(TypeName = "varchar(5)")]
        public string? empGrade { get; set; }

        public int paymentType { get; set; }
        public int bankCode { get; set; }
        public int branchCode { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string? accountNo { get; set; }

        [Column(TypeName = "boolean")]
        public bool? status { get; set; }


        [Column(TypeName = "boolean")]
        public bool? isPaysheetGenerated { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? changeBy { get; set; }
        public DateTime? changeDate { get; set; }
    }
}
