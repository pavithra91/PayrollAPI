using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollAPI.Models.Payroll
{
    public class Special_Tax_Emp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int companyCode { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string epf { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string? costCenter { get; set; }
        public int? location { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string? calFormula { get; set; }

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
        public DateTime lastUpdateDate { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }
}
