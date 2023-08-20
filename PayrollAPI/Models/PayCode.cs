using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayrollAPI.Models
{
    public class PayCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int companyCode { get; set; }
        public int payCode { get; set; }

        [Column(TypeName = "varchar(5)")]
        public string? calCode { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string? type { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? description { get; set; }

        [Column(TypeName = "bool")]
        public bool isTaxableGross { get; set; }

        [Column(TypeName = "decimal(3, 2)")]
        public decimal rate { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? createdBy { get; set; }
        public DateTime createdDate { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? lastUpdateBy { get; set; }
        public DateTime lastUpdateDate { get; set; }
    }
}
