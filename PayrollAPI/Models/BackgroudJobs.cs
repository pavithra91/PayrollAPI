using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayrollAPI.Models
{
    public class BackgroudJobs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int companyCode { get; set; }
        public int period { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string? backgroudJobStatus { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? createdTime { get; set; }
        public DateTime? finishedTime { get; set; }
    }
}
