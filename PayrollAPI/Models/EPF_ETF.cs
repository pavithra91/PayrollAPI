using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollAPI.Models
{
    public class EPF_ETF
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int companyCode { get; set; }
        public int location { get; set; }
        public int period { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string epf { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string? empName { get; set; }

        [Column(TypeName = "varchar(5)")]
        public string? grade { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal emp_contribution  { get; set; }
        
        [Column(TypeName = "decimal(10, 2)")]
        public decimal comp_contribution { get; set; }
        
        [Column(TypeName = "decimal(10, 2)")]
        public decimal etf { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal taxableGross { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal epfGross { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal tax { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? lumpsumTax { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? createdBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdTime { get; set; }
    }
}
