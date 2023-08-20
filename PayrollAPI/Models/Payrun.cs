using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayrollAPI.Models
{
    public class Payrun
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int period { get; set; }
        public int noOfEmployees { get; set; }
        public int noOfRecords { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? approvedBy { get; set; }
        public DateTime approvedDate { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? payrunStatus { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? payrunBy { get; set; }
        public DateTime payrunDate { get; set; }
    }
}
