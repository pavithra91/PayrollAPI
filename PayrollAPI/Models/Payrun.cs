using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollAPI.Models
{
    public class Payrun
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int companyCode { get; set; }
        public int period { get; set; }
        public int noOfEmployees { get; set; }
        public int noOfRecords { get; set; }


        [Column(TypeName = "varchar(10)")]
        public string? dataTransferredBy { get; set; }
        public DateTime dataTransferredDate { get; set; }
        public DateTime dataTransferredTime { get; set; }


        [Column(TypeName = "varchar(10)")]
        public string? approvedBy { get; set; }
        public DateTime? approvedDate { get; set; }
        public DateTime? approvedTime { get; set; }


        [Column(TypeName = "varchar(20)")]
        public string? payrunStatus { get; set; }


        [Column(TypeName = "varchar(10)")]
        public string? payrunBy { get; set; }
        public DateTime? payrunDate { get; set; }
        public DateTime? payrunTime { get; set; }


        [Column(TypeName = "varchar(10)")]
        public string? bankFileCreatedBy { get; set; }
        public DateTime? bankFileCreatedDate { get; set; }
        public DateTime? bankFileCreatedTime { get; set; }
    }
}
