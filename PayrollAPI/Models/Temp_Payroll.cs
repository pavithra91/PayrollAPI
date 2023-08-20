using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayrollAPI.Models
{
    public class Temp_Payroll
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int period { get; set; }
        public int epf { get; set; }
        public float othours { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string? type { get; set; }
        
        public int payCode { get; set; }

        [Column(TypeName = "varchar(5)")]
        public string? calCode { get; set; }

        [Column(TypeName = "bool")]
        public bool paytype { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string? costcenter { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string? payCodeType { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal amount { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal balanceamount { get; set; }
    }
}
