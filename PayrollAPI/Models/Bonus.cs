using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollAPI.Models
{
    public class Bonus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int companyCode { get; set; }
        public int period { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string epf { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal amount { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal tax { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? createdBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdTime { get; set; }
    }
}
