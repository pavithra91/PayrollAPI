using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PayrollAPI.Models
{
    public class Temp_Payroll
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int company { get; set; }

        public int plant { get; set; }

        public int period { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string epf { get; set; }

        [DefaultValue(0)]
        public float othours { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string? payCategory { get; set; } 
        
        public int payCode { get; set; }

        [Column(TypeName = "varchar(5)")]
        public string? calCode { get; set; }

        public char? paytype { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string? costcenter { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string? payCodeType { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal amount { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal balanceamount { get; set; }

        public float epfConRate { get; set; }
        public float taxConRate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdTime { get; set; }
    }
}
