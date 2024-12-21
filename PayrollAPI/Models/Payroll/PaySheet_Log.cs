using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollAPI.Models.Payroll
{
    public class PaySheet_Log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int companyCode { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string epf { get; set; }
        public int period { get; set; }

        [Column(TypeName = "boolean")]
        public bool isPaysheetUploaded { get; set; }
        [Column(TypeName = "boolean")]
        public bool isSMSSend { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string? paysheetID { get; set; }

        [Column(TypeName = "varchar(300)")]
        public string? message { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? changeBy { get; set; }
        public DateTime? changeDate { get; set; }
    }
}
