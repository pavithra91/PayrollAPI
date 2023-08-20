using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollAPI.Models
{
    public class Calculation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int companyCode { get; set; }
        public int sequence { get; set; }
        public int payCode { get; set; }

        [Column(TypeName = "varchar(5)")]
        public string? calCode { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string? type { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string? calFormula { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string? calDescription { get; set; }

        [Column(TypeName = "bool")]
        public bool status { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? createdBy { get; set; }
        public DateTime createdDate { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? lastUpdateBy { get; set; }
        public DateTime lastUpdateDate { get; set; }
    }
}
