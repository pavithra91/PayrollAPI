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

        public int period { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string epf { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string? empName { get; set; }

        [Column(TypeName = "varchar(2)")]
        public char grade { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal emp_contribution  { get; set; }
        
        [Column(TypeName = "decimal(10, 2)")]
        public decimal comp_contribution { get; set; }
        
        [Column(TypeName = "decimal(10, 2)")]
        public decimal etf { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? createdBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdTime { get; set; }
    }
}
