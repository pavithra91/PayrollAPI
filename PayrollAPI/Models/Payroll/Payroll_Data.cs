using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollAPI.Models.Payroll
{
    public class Payroll_Data
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int companyCode { get; set; }
        public int location { get; set; }
        public int period { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string epf { get; set; }
        public float othours { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string? payCategory { get; set; }

        public int payCode { get; set; }

        [Column(TypeName = "varchar(5)")]
        public string? calCode { get; set; }

        public char? paytype { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string? costCenter { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string? payCodeType { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal amount { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal balanceAmount { get; set; }

        public float epfConRate { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal epfContribution { get; set; }

        public float taxConRate { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal taxContribution { get; set; }

        [Column(TypeName = "boolean")]
        public bool? displayOnPaySheet { get; set; }
    }
}
