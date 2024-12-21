using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollAPI.Models.Payroll
{
    public class SysLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public DateTime? loggedDate { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string application { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string level { get; set; }

        [Column(TypeName = "TEXT")]
        public string message { get; set; }

    }
}
