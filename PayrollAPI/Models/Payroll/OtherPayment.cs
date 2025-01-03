using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static PayrollAPI.Data.EntityMapping.StatusMapper;

namespace PayrollAPI.Models.Payroll
{
    public class OtherPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string epf { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string empName { get; set; }
        public string bankCode { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string accountNo { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal amount { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string paymentCategory { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string voucherNo { get; set; }
        public DateTime bankTransferDate { get; set; }
        public PaymentStatus paymentStatus { get; set; }


        // Logs
        [Column(TypeName = "varchar(10)")]
        public string? createdBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdTime { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? lastUpdateBy { get; set; }
        public DateTime? lastUpdateDate { get; set; }
        public DateTime? lastUpdateTime { get; set; }
    }
}
