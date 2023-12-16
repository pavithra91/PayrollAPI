using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollAPI.Models
{
    public class Unrecovered
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int companyCode { get; set; }
        public int location { get; set; }
        public int period { get; set; }
        public string epf { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string? payCategory { get; set; }

        public int payCode { get; set; }

        [Column(TypeName = "varchar(5)")]
        public string? calCode { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string? costcenter { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal amount { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdTime { get; set; }
    }
}
